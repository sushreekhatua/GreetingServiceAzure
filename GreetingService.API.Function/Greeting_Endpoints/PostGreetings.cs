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

namespace GreetingService.API.Function.Greeting_Endpoints
{
    public class PostGreetings
    {
        private readonly ILogger<PostGreetings> _logger;
        public readonly IGreetingRepository _greetingRepository;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };


        public PostGreetings(ILogger<PostGreetings> log, IGreetingRepository greetingrepository1, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _greetingRepository = greetingrepository1;
            Authhandler = _iauthHandler;
        }



        [FunctionName("PostGreetings")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiParameter(name: "PostGreetings", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            var body = await req.ReadAsStringAsync();
            var greetings = JsonSerializer.Deserialize<Greeting>(body,_jsonSerializerOptions);

            //_greetingRepository.Create(new Greeting() { To="Stockholm",
            //                                             From= "Helloöööö",
            //                                                Name="Keen",
            //                                        Message= "mr blobby"});

            if (Authhandler.IsAuthorized(req))
            {
                try
                {
                    await _greetingRepository.CreateAsync(greetings);
                }
                catch
                {
                    return new ConflictResult();
                }

                return new AcceptedResult();
            }
            return new UnauthorizedResult();
           
        }        
    }
}