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
        public readonly IUserService _userservice;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };


        public PutUsers(ILogger<PutUsers> log, IUserService userservice, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _userservice = userservice;
            Authhandler = _iauthHandler;
        }

        [FunctionName("PutUsers")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "PutUsers", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<User>), Description = "The OK response")]


        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "User")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var body = await req.ReadAsStringAsync();
            var users = System.Text.Json.JsonSerializer.Deserialize<User>(body, _jsonSerializerOptions);

            if (Authhandler.IsAuthorized(req))
            {

                try
                {
                    await _userservice.UpdateAsync(users);
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
