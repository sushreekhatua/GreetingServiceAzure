using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core.Entities;
using GreetingService.Core.Enums;
using GreetingService.Core.Exceptions;
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
        //public readonly IGreetingRepository _greetingRepository;
        private readonly IMessagingService _messagingService;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };
        

        public PostGreetings(ILogger<PostGreetings> log, IMessagingService messagingService, IAuthHandler _iauthHandler)
        {
            _logger = log;
           _messagingService = messagingService;
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




            //_greetingRepository.Create(new Greeting() { To="Stockholm",
            //                                             From= "Helloöööö",
            //                                                Name="Keen",
            //                                        Message= "mr blobby"});
           
            if (Authhandler.IsAuthorized(req))
            {
                Greeting greetings;

                try
                {
                    var body = await req.ReadAsStringAsync();
                    greetings = JsonSerializer.Deserialize<Greeting>(body, _jsonSerializerOptions);
                    //await _greetingRepository.CreateAsync(greetings);
                }
                //catch (InvalidEmailException e)
                catch (Exception e)
                {
                   
                    return new BadRequestObjectResult(e.Message);
                }
                try
                {
                    await _messagingService.SendAsync(greetings, MessagingServiceSubject.NewGreeting);
                }

                catch
                {
                    return new ConflictResult();
                }

                //AcceptedObject Result
                return new AcceptedResult();
            }
            return new UnauthorizedResult();
           
        }        
    }
}