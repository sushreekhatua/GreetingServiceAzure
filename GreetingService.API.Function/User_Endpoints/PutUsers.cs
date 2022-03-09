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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.API.Function.User_Endpoints
{
    public class PutUsers
    {
        private readonly ILogger<PutUsers> _logger;
        //public readonly IUserService _userservice;
        private readonly IMessagingService _messagingService;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };


        public PutUsers(ILogger<PutUsers> log, IMessagingService messagingService, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _messagingService = messagingService;
            Authhandler = _iauthHandler;
        }

        [FunctionName("PutUsers")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "PutUsers", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<User>), Description = "The OK response")]


        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "User/{email}")] HttpRequest req,string email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            

            if (Authhandler.IsAuthorized(req))
            {
                User user1;
                try
                {
                    
                    var body = await req.ReadAsStringAsync();
                    user1 = JsonSerializer.Deserialize<User>(body, _jsonSerializerOptions);
                    //await _userservice.UpdateAsync(users);
                    //return new AcceptedResult();
                }
                //catch (InvalidEmailException e)
                catch(Exception e)
                {

                    return new BadRequestObjectResult(e.Message);
                }
                try
                {
                    await _messagingService.SendAsync(user1, MessagingServiceSubject.updateuser);
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
