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
        //public async Task CreateAsync(Greeting greeting)
        //{
        //    //var blob = _blobContainerClient.GetBlobClient(greeting.Id.ToString());              //get a reference to the blob using Greeting.ID as blob name
        //    var blobclient = _blobContainerClient.GetBlobClient(blobname);
        //    if (await blobclient.ExistsAsync())
        //    {
        //        var content = await blobclient.DownloadContentAsync();
        //        var greetings=content.Value.Content.ToObjectFromJson<List<Greeting>>();

        //        if (greetings.Any(x => x.Id == greeting.Id))
        //            throw new Exception($"Greeting with id: {greeting.Id} already exists");

        //        greetings.Add(greeting) ;

        //        await blobclient.DeleteAsync();
        //        var binarydata = new BinaryData(greetings);
        //        await blobclient.UploadAsync(binarydata);


        //    }
        //   else
        //    {
        //        var binarydata = new BinaryData(new List<Greeting>() { greeting});
        //        await blobclient.UploadAsync(binarydata);
        //    }           
        //}

        public async Task CreateAsync(Greeting greeting)   // for many blobs
        {
            var pathname = $"{greeting.From}/{greeting.To}/{greeting.Id}";
            var blob = _blobContainerClient.GetBlobClient(pathname);
            //var blob = _blobContainerClient.GetBlobClient(greeting.Id.ToString());              //get a reference to the blob using Greeting.ID as blob name
            if (await blob.ExistsAsync())
                throw new Exception($"Greeting with id: {greeting.Id} already exists");

            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blob.UploadAsync(greetingBinary);
        }


        //Get greetings method
        //public async Task<IEnumerable<Greeting>> CreateAsync()       // for single blob
        //{            
        //     var blobclient = _blobContainerClient.GetBlobClient(blobname);
        //     var blobcontent = await blobclient.DownloadContentAsync();
        //     var greeting = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();
        //     return greeting;                      
        //}

        public async Task<IEnumerable<Greeting>> CreateAsync()   //For many blobs
        {
            var greetings = new List<Greeting>();
            var blobs = _blobContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                var blobContent = await blobClient.DownloadContentAsync();
                var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
                greetings.Add(greeting);
            }

            return greetings;
        }


        //To delete the greeting with id


        //public async Task DeleteAsync(Guid id)   // for many blobs
        //{
        //    var blobClient = _blobContainerClient.GetBlobClient(blobname);
        //    var blobcontent = await blobClient.DownloadContentAsync();
        //    var greetings = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();
            
        //    var deletegreeting=greetings.FirstOrDefault(i => i.Id == id);
        //    if (deletegreeting == null)
        //        throw new Exception($"Greeting with id: {id} not found");
        //    else
        //    {
        //        greetings.Remove(deletegreeting);
        //    }
        //    await blobClient.DeleteAsync();
        //    var greetingBinary = new BinaryData(greetings, _jsonSerializerOptions);
        //    await blobClient.UploadAsync(greetingBinary);

        //}

        public async Task DeleteAsync(Guid id)   // for many blobs
        {
            var newid = id.ToString();

            var blobs = _blobContainerClient.GetBlobsAsync();
            var newblob = await blobs.FirstOrDefaultAsync(x => x.Name.Contains(newid));
            //var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
           
            if(newblob == null)
            {
                return;
            }

            var blobClient = _blobContainerClient.GetBlobClient(newblob.Name);
           await blobClient.DeleteAsync();            

        }
    


        //To get the greeting with id

        //public async Task<Greeting> GetAsync(Guid id)              // for single blob containing all greetings
        //{

        //    var blobclient = _blobContainerClient.GetBlobClient(blobname);
        //    var blobcontent = await blobclient.DownloadContentAsync();
        //    var greetings = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();

        //    if (!greetings.Exists(x => x.Id == id))
        //         throw new Exception($"Greeting with id: {id} not found");
        //    return greetings.FirstOrDefault(x => x.Id == id);


        //}

        public async Task<Greeting> GetAsync(Guid id)                          //for many blobs
        {
            var newid = id.ToString();
            var blobs = _blobContainerClient.GetBlobsAsync();
            var newblob = await blobs.FirstOrDefaultAsync(x => x.Name.Contains(newid));
            //var blobClient = _blobContainerClient.GetBlobClient(id.ToString());
            var blobClient = _blobContainerClient.GetBlobClient(newblob.Name);

            
            if (!await blobClient.ExistsAsync())
                throw new Exception($"Greeting with id: {id} not found");

            var blobContent = await blobClient.DownloadContentAsync();
            var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
            return greeting;
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var prefix = "";                            //A prefix is literally a prefix on the name, that means it starts from the left. Our blob names are stored like this: {from}/{to}/{id}
            if (!string.IsNullOrWhiteSpace(from))       //only add 'from' to prefix if it's not null
            {
                prefix = from;
                if (!string.IsNullOrWhiteSpace(to))     //only add 'to' to prefix if it's not null and 'from' is not null
                {
                    prefix = $"{prefix}/{to}";          //no wild card support in prefix, only add 'to' to prefix if 'from' also is not null
                }
            }

            var blobs = _blobContainerClient.GetBlobsAsync(prefix: prefix);             //send prefix to the server to only retrieve blobs that matches. The below logic would work even without prefix, but it's slightly optimized if we can send a non empty prefix

            var greetings = new List<Greeting>();
            await foreach (var blob in blobs)                                           //this is how we can asynchronously iterate and process data in an IAsyncEnumerable<T>
            {
                var blobNameParts = blob.Name.Split('/');

                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}/{to}/"))    //both 'from' and 'to' has values
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}"))      //'from' has value, 'to' is null
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to) && blobNameParts[1].Equals(to))          //'from' is null, 'to' has value
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))                                          //both 'from' and 'to' are null
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
            }

            return greetings;
        }

        private async Task<Greeting> DownloadBlob(Azure.Storage.Blobs.Models.BlobItem blob)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
            var blobContent = await blobClient.DownloadContentAsync();              //downloading lots of blobs like this will be slow, a more common scenario would be to list metadata for each blob and then download one or more blobs on demand instead of by default downloading all blobs. But we'll roll with this solution in this exercise
            var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
            return greeting;
        }


        //To update greeting


        //public async Task UpdateAsync(Greeting newgreeting)                               //for single blob
        //{


        //    var blobclient = _blobContainerClient.GetBlobClient(blobname);

        //    var blobcontent = await blobclient.DownloadContentAsync();
        //    var greetings = blobcontent.Value.Content.ToObjectFromJson<List<Greeting>>();



        //    var existingGreeting = greetings.FirstOrDefault(x => x.Id == newgreeting.Id);

        //    if (existingGreeting == null)
        //        throw new Exception($"Greeting with id: {newgreeting.Id} not found");

        //    existingGreeting.To = newgreeting.To;
        //    existingGreeting.From = newgreeting.From;
        //    existingGreeting.Message = newgreeting.Message;
        //    await blobclient.DeleteAsync();

        //    var binarydata = new BinaryData(greetings);
        //    await blobclient.UploadAsync(binarydata);
        //}

        public async Task UpdateAsync(Greeting greeting)                             // for many blobs
        {

            var oldgreeting=await GetAsync(greeting.Id);  


            var oldgreetingpath = $"{oldgreeting.From}/{oldgreeting.To}/{oldgreeting.Id}";
            var blobClient = _blobContainerClient.GetBlobClient(oldgreetingpath);
            //var blobClient = _blobContainerClient.GetBlobClient(greeting.Id.ToString());
            await blobClient.DeleteIfExistsAsync();

            var newgreetingpath= $"{greeting.From}/{greeting.To}/{greeting.Id}";            
            var newgreetingblobclient = _blobContainerClient.GetBlobClient(newgreetingpath);
            var newgreetingBinary = new BinaryData(greeting, _jsonSerializerOptions);

            //var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await newgreetingblobclient.UploadAsync(newgreetingBinary);
        }











    }
}
