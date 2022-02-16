using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class BlobGreetingRepository : IGreetingRepository
    {
        private const string blobname="Greetingblob.json";
        private const string _blobContainerName = "greetings";
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
            var blobclient = _blobContainerClient.GetBlobClient(blobname);
            if (await blobclient.ExistsAsync())
            {
                var content = await blobclient.DownloadContentAsync();
                var greetings=content.Value.Content.ToObjectFromJson<List<Greeting>>();
                 
                if (greetings.Any(x => x.Id == greeting.Id))
                    throw new Exception($"Greeting with id: {greeting.Id} already exists");

                greetings.Add(greeting) ;
                
                await blobclient.DeleteAsync();
                var binarydata = new BinaryData(greetings);
                await blobclient.UploadAsync(binarydata);

                
            }
           else
            {
                var binarydata = new BinaryData(new List<Greeting>() { greeting});
                await blobclient.UploadAsync(binarydata);
            }           
        }

        //public async Task CreateAsync(Greeting greeting)
        //{
        //    var blob = _blobContainerClient.GetBlobClient(greeting.Id.ToString());              //get a reference to the blob using Greeting.ID as blob name
        //    if (await blob.ExistsAsync())
        //        throw new Exception($"Greeting with id: {greeting.Id} already exists");

        //    var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
        //    await blob.UploadAsync(greetingBinary);
        //}


        //Get greetings method
        public async Task<IEnumerable<Greeting>> CreateAsync()
        {            
             var blobclient = _blobContainerClient.GetBlobClient(blobname);
             var blobcontent = await blobclient.DownloadContentAsync();
             var greeting = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();
             return greeting;                      
        }

        //public async Task<IEnumerable<Greeting>> CreateAsync()
        //{
        //    var greetings = new List<Greeting>();
        //    var blobs = _blobContainerClient.GetBlobsAsync();
        //    await foreach (var blob in blobs)
        //    {
        //        var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
        //        var blobContent = await blobClient.DownloadContentAsync();
        //        var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
        //        greetings.Add(greeting);
        //    }

        //    return greetings;
        //}


        //To delete the greeting with id
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }


        //To get the greeting with id
        public async Task<Greeting> GetAsync(Guid id)
        {
            
            var blobclient = _blobContainerClient.GetBlobClient(blobname);
            var blobcontent = await blobclient.DownloadContentAsync();
            var greetings = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();

            if (!greetings.Exists(x => x.Id == id))
                 throw new Exception($"Greeting with id: {id} not found");
            return greetings.FirstOrDefault(x => x.Id == id);


        }

        //public async Task<Greeting> GetAsync(Guid id)
        //{
        //    var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
        //    if (!await blobClient.ExistsAsync())
        //        throw new Exception($"Greeting with id: {id} not found");

        //    var blobContent = await blobClient.DownloadContentAsync();
        //    var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
        //    return greeting;
        //}



        //To update greeting
        public async Task UpdateAsync(Greeting newgreeting)
        {


            var blobclient = _blobContainerClient.GetBlobClient(blobname);

            var blobcontent = await blobclient.DownloadContentAsync();
            var greetings = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();


           
            var existingGreeting = greetings.FirstOrDefault(x => x.Id == newgreeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {newgreeting.Id} not found");

            existingGreeting.To = newgreeting.To;
            existingGreeting.From = newgreeting.From;
            existingGreeting.Message = newgreeting.Message;                         
            
            var binarydata = new BinaryData(greetings);
            await blobclient.UploadAsync(binarydata);
        }

        //public async Task UpdateAsync(Greeting greeting)
        //{
        //    var blobClient = _blobContainerClient.GetBlobClient(greeting.Id.ToString());
        //    await blobClient.DeleteIfExistsAsync();
        //    var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
        //    await blobClient.UploadAsync(greetingBinary);
        //}











    }
}
