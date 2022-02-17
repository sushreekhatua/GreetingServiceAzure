
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft;

namespace GreetingService.API.Function
{
    public class GetGreetings
    {
        private readonly ILogger<GetGreetings> _logger;
        private  IAuthHandler Authhandler {get; set;}
        public readonly IGreetingRepository _greetingRepository;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };

        public GetGreetings(ILogger<GetGreetings> log, IGreetingRepository greetingrepository1,IAuthHandler _iauthHandler)
        {
            _logger = log;
            _greetingRepository = greetingrepository1;
            Authhandler = _iauthHandler;
        }



        [FunctionName("GetGreetings")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!Authhandler.IsAuthorized(req))
                return new UnauthorizedResult();


            try
            {
                var from = req.Query["from"];
                var to = req.Query["to"];
                var greetings = await _greetingRepository.GetAsync(from,to);
                return new OkObjectResult(greetings);
            }
            catch
            {
                return new NotFoundResult();
            }



            

        }

    }   
}

