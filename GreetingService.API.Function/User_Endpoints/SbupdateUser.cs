using System;
using System.Threading.Tasks;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.User_Endpoints
{
    public class SbupdateUser
    {
        private readonly ILogger<SbupdateUser> _logger;
        private readonly IUserService _userservice;

        public SbupdateUser(ILogger<SbupdateUser> log, IUserService userservice)
        {
            _logger = log;
            _userservice = userservice;
        }

        [FunctionName("SbupdateUser")]
        public async Task Run([ServiceBusTrigger("main", "user_update", Connection = "ServiceBusConnectionString")]User user)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {user}");
            try
            {
                await _userservice.UpdateAsync(user);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to insert Greeting to IGreetingRepository", e);
                throw;
            }
        }
    }
}
