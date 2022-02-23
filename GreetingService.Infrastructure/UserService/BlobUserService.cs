using Azure.Storage.Blobs;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.UserService
{
    public class BlobUserService : IUserService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string _blobContainerName="users";
        public readonly string _blobname = "users.json";
        private readonly ILogger<BlobUserService> _logger;


        public BlobUserService(IConfiguration configuration, ILogger<BlobUserService> logger)
        {
            var connectionString = configuration["LoggingStorageAccount"];          //get connection string from our app configuration
            _blobContainerClient = new BlobContainerClient(connectionString, _blobContainerName);
            _blobContainerClient.CreateIfNotExists();
            _logger = logger;

        }
        public bool IsValidUser(string username, string password)
        {
            var blobclient = _blobContainerClient.GetBlobClient(_blobname);
            if (!blobclient.Exists())
            {
                _logger.LogWarning("Invalid credentials for {username}", username);
                return false;
            }
            var content= blobclient.DownloadContent();
            var userdictionary =content.Value.Content.ToObjectFromJson<Dictionary<string,string>>();
            
            if (userdictionary.TryGetValue(username, out var storedPassword))
            {
                if (storedPassword == password)
                {
                    _logger.LogInformation("Valid credentials for {username}", username);
                    return true;
                }
            }
            return false;
            
        }

        

        public async Task<IEnumerable<User>> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        
        public async Task<User> GetAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
