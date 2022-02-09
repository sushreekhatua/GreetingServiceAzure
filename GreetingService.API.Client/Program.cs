// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Diagnostics;

namespace GreetingService.API.Client
{
    public class Program
    {
        private static HttpClient _httpClient = new();
        private const string getmsg = "get greetings";
        private const string getmsgbyid = "get greeting by id";
        private const string postmsg = "write greeting";
        private const string putmsg = "update greeting";
        private const string deletemsg = "delete greeting";
        private const string _repeatingCallsCommand = "repeat calls ";

        private static string _from = "noman";
        private static string _to = "Soman";
       

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true,PropertyNameCaseInsensitive=true, DefaultIgnoreCondition= System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault};


        static async Task Main(string[] args)
        {
            var authParam = Convert.ToBase64String(Encoding.UTF8.GetBytes("keen:winter2022"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authParam);        //Always send this header for all requests from this HttpClient
            //_httpClient.BaseAddress = new Uri("http://localhost:5071/");                                                                                                                                  //_httpClient.BaseAddress = new Uri("http://localhost:5020/");
            //_httpClient.BaseAddress = new Uri("https://sadhana-appservice-dev.azurewebsites.net/");
            _httpClient.BaseAddress = new Uri("https://sadhana-function-dev.azurewebsites.net/");


            Console.WriteLine("Welcome to command line Greeting client");
            Console.WriteLine("Enter name of greeting sender: ");
            var from = Console.ReadLine();
            
            Console.WriteLine("Enter the name of Recepient: ");
            var to = Console.ReadLine();
            


            if (!string.IsNullOrWhiteSpace(from))
                _from = from;

            
            if (!string.IsNullOrWhiteSpace(to))
                _to = to;





            Console.WriteLine("Available commands:");
            Console.WriteLine(getmsg);
            Console.WriteLine($"{getmsgbyid}");
            Console.WriteLine($"{postmsg}");
            Console.WriteLine($"{putmsg}");
            Console.WriteLine($"{deletemsg}");
            Console.WriteLine($"{_repeatingCallsCommand} [count]");


            while (true)
            {
                Console.WriteLine("\nWrite command and press [enter] to execute");
                var command = Console.ReadLine();
                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine("You entered nothing.Try again!!");
                    continue;
                }
                else
                {
                    if (command.Equals(getmsg, StringComparison.OrdinalIgnoreCase))
                    {
                        await GetGreetingsAsync();
                        break;
                    }
                    else if (command.Equals(getmsgbyid, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter a id to get the API: ");
                        var inputid = Console.ReadLine();

                        if (Guid.TryParse(inputid, out var newid))
                        {
                            await GetGreetingAsync(newid);
                            break;
                        }
                        else 
                        { 
                            Console.WriteLine("Provided id is not valid.Try again!!");
                            continue;
                        }

                    }
                    else if (command.Equals(postmsg, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter a message to post it on the API");
                        var msgs = Console.ReadLine();
                        await WriteGreetingAsync(msgs);
                        break;

                    }

                    else if (command.Equals(putmsg, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter a message to post it on the API");
                        var messagePart = Console.ReadLine();
                        Console.WriteLine("Enter a vaild Id to update the API");
                        var idPart = Console.ReadLine();


                        if (Guid.TryParse(idPart, out var id))
                        {
                            await UpdateGreetingAsync(id, messagePart);
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"{idPart} is not a valid GUID. Try again!!");
                            continue;
                        }
                    }

                    else if (command.Equals(deletemsg, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter a valid ID to delete it on the API");
                        var idPart = Console.ReadLine();
                        


                        if (Guid.TryParse(idPart, out var id))
                        {
                            await DeleteGreetingAsync(id);
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"{idPart} is not a valid GUID. Try again!!");
                            continue;
                        }
                    }
                    else if (command.StartsWith(_repeatingCallsCommand))
                    {
                        var countPart = command.Replace(_repeatingCallsCommand, "");

                        if (int.TryParse(countPart.Trim(), out var count))
                        {
                            await RepeatCallsAsync(count);
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Could not parse {countPart} as int");
                        }
                    }

                    else 
                    {
                        Console.WriteLine("Try again .");
                        continue;
                       
                    }
                }
            }

            //await XmlGreetingAsync();
            Console.ReadLine();
           

            
        }

        //To Get 
        private static async Task<IEnumerable<Greeting>> GetGreetingsAsync()
        {
            try
            {
                /*
                 By Keen
                  */
                //var request = new HttpRequestMessage(HttpMethod.Get, "api/Greeting");
                //request.Headers.Add("Accept", "application/xml");
                //var response = await _httpClient.SendAsync(request);

                
                var response = await _httpClient.GetAsync("api/Greeting");
                var content = await response.Content.ReadAsStringAsync();
                var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine(response.EnsureSuccessStatusCode());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("We will get our greetings.");
                Console.ResetColor();
                Console.WriteLine(" ");
                foreach (var greeting in greetings)
                {
                    Console.WriteLine($"[{greeting.message}] , [{greeting.from}]  - [{greeting.to} ] -> [{greeting.id}]");
                }
                Console.WriteLine("____________________________________");
                Console.WriteLine();
                return greetings;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Enumerable.Empty< Greeting >() ;
            
        }



        //To Get By Id
        private static async Task GetGreetingAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Greeting/{id}");
                var content = await response.Content.ReadAsStringAsync();
                var greeting7 = JsonSerializer.Deserialize<Greeting>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine(response.EnsureSuccessStatusCode());
                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{greeting7.message} , {greeting7.from} , {greeting7.to} , {greeting7.id}");
                Console.ResetColor();
                Console.WriteLine("____________________________________");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
           
        }


        // To Post
        private static async Task WriteGreetingAsync(string message)
        {


            try
            {
                var greeting = new Greeting
                {
                    
                    from =_from,
                    name="Blippi",
                    to=_to,
                    message = message,
                    id = Guid.NewGuid(),
                    
                };
                var response = await _httpClient.PostAsJsonAsync("api/Greeting", greeting);
                if (response.IsSuccessStatusCode == false)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                Console.WriteLine($"Wrote greeting. Service responded with: {response.StatusCode}");            //all HTTP responses always contain a status code
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Write greeting failed: {e.Message}\n");
            }


            //var greetingx = new Greeting()
            //{
            //    From = "Hello22",
            //    To = "Someone",
            //    Message = message1
            //};
            //var greetingsent= await _httpClient.PutAsJsonAsync("api/Greeting", greetingx);
            ////var serialized= JsonSerializer.Serialize(greetingx, _jsonSerializerOptions);
            ////var content = new StringContent(serialized,null,"application/json");
            ////var greetingsent1 = await _httpClient.PostAsync("api/Greeting",content);
            //Console.WriteLine(greetingsent.EnsureSuccessStatusCode());
            //Console.WriteLine(" ");
            //Console.ForegroundColor = ConsoleColor.Blue;
            //Console.WriteLine("We have posted a new greeting..");
            //Console.ResetColor();
            //Console.WriteLine("____________________________________");

        }



        //To Put or update
        private static async Task UpdateGreetingAsync(Guid id1, string messagePart)
        {
            try
            {
                var greeting = new Greeting
                {
                    id = id1,
                    from = "_from",
                    to = "_to",
                    message = messagePart,
                    name="Hej"
                };
                var response = await _httpClient.PutAsJsonAsync("api/Greeting", greeting);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Updated greeting. Service responded with: {response.StatusCode}");
                Console.ResetColor();
                Console.WriteLine("____________________________________");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Update greeting failed: {e.Message}\n");

            }
            
        }

        //To Delete
        private static async Task DeleteGreetingAsync(Guid id)
        {
            try
            {

                
                var response1 = await _httpClient.DeleteAsync($"api/Greeting/{id}");
                
                Console.WriteLine(response1.EnsureSuccessStatusCode());
                Console.WriteLine(" ");
                Console.ForegroundColor= ConsoleColor.DarkMagenta;
                Console.WriteLine("Greeting is deleted.");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Update greeting failed: {e.Message}\n");

            }
        }



        //To serialize the List<Greeting> to xml file
        private static async Task XmlGreetingAsync()
        {
            var response = await _httpClient.GetAsync("api/Greeting");
            Console.WriteLine(response.EnsureSuccessStatusCode());
            var responsebody=await response.Content.ReadAsStringAsync();
            var greetings=JsonSerializer.Deserialize<List<Greeting>>(responsebody);


            var filename = "greetingasync.xml";

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
            };
            using var xmlWriter = XmlWriter.Create(filename, xmlWriterSettings);
            var serializer = new XmlSerializer(typeof(List<Greeting>));                             //this xml serializer does not support serializing interfaces, need to convert to a concrete class
            serializer.Serialize(xmlWriter, greetings.ToList());                                   //convert our greetings of type IEnumerable (interface) to List (concrete class)


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Wrote {greetings.Count()} greeting(s) to {filename}");
            Console.ResetColor();
        }


        private static async Task RepeatCallsAsync(int count)
        {
            var greetings = await GetGreetingsAsync();
            var greeting = greetings.First();
            //var stopwatch = Stopwatch.StartNew();
            //init a jobs list
            var jobs = new List<int>();
            for (int i = 0; i < count; i++)
            {
                jobs.Add(i);

                //var start = stopwatch.ElapsedMilliseconds;
                //var response = await _httpClient.GetAsync($"api/greeting/{greeting.id}");
                //var end = stopwatch.ElapsedMilliseconds;

                //Console.WriteLine($"Response: {response.StatusCode} - Call: {job} - latency: {end - start} ms - rate/s: {job / stopwatch.Elapsed.TotalSeconds}");

            }

            var stopwatch = Stopwatch.StartNew();           //use stopwatch to measure elapsed time just like a real world stopwatch

            //I cheat by running multiple calls in parallel for maximum throughput - we will be limited by our cpu, wifi, internet speeds
            //This is a bit advanced and the syntax is new with lamdas - don't worry if you don't understand all of it.
            //I always copy this from the internet and adapt to my needs
            //Running this in Visual Studio debugger is slow, try running .exe file directly from File Explorer or command line prompt
            
            
            //we can change { MaxDegreeOfParallelism = 50 }

            await Parallel.ForEachAsync(jobs, new ParallelOptions { MaxDegreeOfParallelism = 100 }, async (job, token) =>
            {
                var start = stopwatch.ElapsedMilliseconds;
                var response = await _httpClient.GetAsync($"api/greeting/{greeting.id}");
                var end = stopwatch.ElapsedMilliseconds;

                Console.WriteLine($"Response: {response.StatusCode} - Call: {job} - latency: {end - start} ms - rate/s: {job / stopwatch.Elapsed.TotalSeconds}");
                
            });
        }



    }
}
