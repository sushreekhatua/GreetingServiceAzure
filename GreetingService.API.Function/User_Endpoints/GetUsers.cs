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

namespace GreetingService.API.Function.User_Endpoints
{
    public class GetUsers
    {
        private readonly ILogger<GetUsers> _logger;
        private IAuthHandler Authhandler { get; set; }
        public readonly IUserService _userservice;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };

        public GetUsers(ILogger<GetUsers> log, IUserService userservice, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _userservice = userservice;
            Authhandler = _iauthHandler;
        }



        [FunctionName("GetUsers")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Core.Entities.User>), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (Authhandler.IsAuthorized(req))
            {
                try
                {
                    var from = req.Query["from"];
                    var to = req.Query["to"];
                    var users = await _userservice.ReadAsync();
                    //var users = await _userservice.GetAsync(from, to);
                    return new OkObjectResult(users);
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

