using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;
using GreetingService.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class ConvertGreetingToCsv
    {
        [FunctionName("ConvertGreetingToCsv")]
        public async Task Run([BlobTrigger("greetings/{name}", Connection = "LoggingStorageAccount")]Stream greetingJsonblob, [Blob("greetings-csv/{name}", FileAccess.Write, Connection = "LoggingStorageAccount")] Stream greetingCsvBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {greetingJsonblob.Length} Bytes");

            //var greetings = JsonSerializer.Deserialize<List<Greeting>>(greetingJsonblob);
            var greeting = JsonSerializer.Deserialize<Greeting>(greetingJsonblob);
            var streamwriter=new StreamWriter(greetingCsvBlob);
            streamwriter.WriteLine("id;from;to;message;timestamp");
            streamwriter.WriteLine($"{greeting.Id};{greeting.From};{greeting.To};{greeting.Message};{greeting.Time}");
            //foreach (var greeting in greetings)
            //{
            //    streamwriter.WriteLine($"{greeting.Id};{greeting.From};{greeting.To};{greeting.Message};{greeting.Time}");
            //}
            
            await streamwriter.FlushAsync();
        }
        
    }
}
