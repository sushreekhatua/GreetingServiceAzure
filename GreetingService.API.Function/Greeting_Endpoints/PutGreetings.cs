using System.Collections.Generic;
using System.IO;
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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function.Greeting_Endpoints
{
    public class PutGreetings
    {
        private readonly ILogger<PutGreetings> _logger;
        public readonly IGreetingRepository _greetingRepository;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true,PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };
    

        public PutGreetings(ILogger<PutGreetings> log, IGreetingRepository greetingrepository1, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _greetingRepository = greetingrepository1;
            Authhandler = _iauthHandler;
        }

        [FunctionName("PutGreetings")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "PutGreetings", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]


        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "User/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var body = await req.ReadAsStringAsync();
            var greetings = System.Text.Json.JsonSerializer.Deserialize<Greeting>(body, _jsonSerializerOptions);

            if (Authhandler.IsAuthorized(req))
            {

                try
                {
                    await _greetingRepository.UpdateAsync(greetings);
                    return new AcceptedResult();
                }
                catch
                {
                    return new NotFoundResult();
                }
            }
            return new UnauthorizedResult();
            
        }            
        
    }
}
