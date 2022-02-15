using Azure.Storage.Blobs;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class BlobGreetingRepository : IGreetingRepository
    {
        private const string blobname="Greetingblob.json";
        private const string _blobContainerName = "Greetingblob";
        private readonly BlobContainerClient _blobContainerClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        // Get a connection string to our Azure Storage account.  You can
        // obtain your connection string from the Azure Portal (click
        // Access Keys under Settings in the Portal Storage account blade)
        // or using the Azure CLI with:
        //
        //     az storage account show-connection-string --name <account_name> --resource-group <resource_group>
        //
        // And you can provide the connection string to your application
        // using an environment variable.
        public BlobGreetingRepository(IConfiguration configuration)
        {
            var connectionString = configuration["LoggingStorageAccount"];          //get connection string from our app configuration
            _blobContainerClient = new BlobContainerClient(connectionString, _blobContainerName);
            _blobContainerClient.CreateIfNotExists();

        }


        //Create greeting method
        public async Task CreateAsync(Greeting greeting)
        {
            //var blob = _blobContainerClient.GetBlobClient(greeting.Id.ToString());              //get a reference to the blob using Greeting.ID as blob name
            var blob = _blobContainerClient.GetBlobClient(blobname);
            if (await blob.ExistsAsync())
            {
                var content = File.ReadAllText(blobname);
                var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
                if (greetings.Any(x => x.Id == greeting.Id))
                    throw new Exception($"Greeting with id: {greeting.Id} already exists");

                greetings.Add(new Greeting { From="Sadhana",To="keen",Message="hello Keen!"});
                File.WriteAllText(blobname, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
            }
                

            //var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            //await blob.UploadAsync(greetingBinary);

            else
            {
                
                
                File.WriteAllText(blobname, "[]");
               
                
            }
            await blob.UploadAsync(blobname);
        }

       //Get greetings method
        public async Task<IEnumerable<Greeting>> CreateAsync()
        {
            var greetings= new List<Greeting>();
            var blobs=_blobContainerClient.GetBlobsAsync();
            await foreach(var blob in blobs)
            {
                var blobclient = _blobContainerClient.GetBlobClient(blob.Name);
                var blobcontent=await blobclient.DownloadContentAsync();
                var greeting=blobcontent.Value.Content.ToString();
                
                var greeting1 = JsonSerializer.Deserialize<IList<Greeting>>(greeting);
                greetings.Add((Greeting)greeting1);
               
            }
            return greetings;
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var greetings = new List<Greeting>();
            var blobs = _blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var blobclient = _blobContainerClient.GetBlobClient(blobname);
                var blobcontent = await blobclient.DownloadContentAsync();
                var greeting = blobcontent.Value.Content.ToString();

                var greeting1 = JsonSerializer.Deserialize<IList<Greeting>>(greeting);
                greetings.Add((Greeting)greeting1);
                if (!greetings.Exists(x => x.Id == id))
                    throw new Exception($"Greeting with id: {id} not found");

            }
            return greetings?.FirstOrDefault(x => x.Id == id);
        }

        public async Task UpdateAsync(Greeting newgreeting)
        {
           
            
             var blobclient = _blobContainerClient.GetBlobClient(blobname);
               
             var blobcontent = await blobclient.DownloadContentAsync();
             var greeting1 = blobcontent.Value.Content.ToString();

            var content = File.ReadAllText(greeting1);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var existingGreeting = greetings.FirstOrDefault(x => x.Id == newgreeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {newgreeting.Id} not found");

            existingGreeting.To = newgreeting.To;
            existingGreeting.From = newgreeting.From;
            existingGreeting.Message = newgreeting.Message;
            File.WriteAllText(blobname, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
            await blobclient.UploadAsync(blobname);

        }
            
            
            






            
        
    }
}
