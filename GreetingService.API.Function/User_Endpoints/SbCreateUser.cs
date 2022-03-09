using System;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.User_Endpoints
{
    public class SbCreateUser
    {
        private readonly ILogger<SbCreateUser> _logger;
        private readonly IUserService _userservice;

        public SbCreateUser(ILogger<SbCreateUser> log,IUserService userservice)
        {
            _logger = log;
            _userservice = userservice;
        }

        [FunctionName("SbCreateUser")]
        public async Task Run([ServiceBusTrigger("main", "user_create", Connection = "ServiceBusConnectionString")]string user)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {user}");
            var g= JsonSerializer.Deserialize<User>(user);

            try
            {
                var existinguser = await _userservice.GetAsync(g.Email);
                if (existinguser == null)
                    await _userservice.CreateAsync(g);


                //throw new Exception($"The id with {g.Id} already exists");

            }
            catch (Exception e)
            {
                _logger.LogError("Failed to insert user to IGreetingRepository", e);
                throw;
            }
        }
    }
}
