using System;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.Greeting_Endpoints
{
    public class SbCreateGreeting
    {
        private readonly ILogger<SbCreateGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;

        public SbCreateGreeting(ILogger<SbCreateGreeting> log, IGreetingRepository greetingRepository)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
        }

        [FunctionName("SbCreateGreeting")]
        public async Task Run([ServiceBusTrigger("main", "greeting_create", Connection = "ServiceBusConnectionString")] string greeting)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {greeting}");
            
            var g=JsonSerializer.Deserialize<Greeting> (greeting);
            
            try
            {
                var existinggreeting = await _greetingRepository.GetAsync(g.Id);
                if(existinggreeting==null)
                    await _greetingRepository.CreateAsync(g);

                await _greetingRepository.UpdateAsync(g);
                //throw new Exception($"The id with {g.Id} already exists");
                
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to insert Greeting to IGreetingRepository", e);
                throw;
            }
        }
    }
}
