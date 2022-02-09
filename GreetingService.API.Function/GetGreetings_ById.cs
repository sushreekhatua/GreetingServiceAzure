using System;
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
using Newtonsoft.Json;

namespace GreetingService.API.Function
{
    public class GetGreetings_ById
    {
        private readonly ILogger<GetGreetings> _logger;
        public readonly IGreetingRepository _greetingRepository;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };
        private Guid new_id;

        public GetGreetings_ById(ILogger<GetGreetings> log, IGreetingRepository greetingrepository1, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _greetingRepository = greetingrepository1;
            Authhandler = _iauthHandler;
        }



        [FunctionName("GetGreetings_ById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
       // [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Greeting/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //var body = await req.ReadAsStringAsync();
            ////var greetings = JsonSerializer.Deserialize<Greeting>(body, _jsonSerializerOptions);

            //var greetings = JsonConvert.DeserializeObject<Greeting>(body);

            //greetings.Id = id;

            if (!Authhandler.IsAuthorized(req))
                return new UnauthorizedResult();

            try
            {
                Guid.TryParse(id, out new_id);
                var greeting = _greetingRepository.Get(new_id);
                return new OkObjectResult(greeting);
            }
            catch
            {
                return new NotFoundResult();
            }
            
        }

    }
}
