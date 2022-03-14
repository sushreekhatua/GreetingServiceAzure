using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core.Entities;
using GreetingService.Core.Exceptions;
using GreetingService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GreetingService.API.Function.User_Endpoints
{
    public class ApproveUser
    {
        private readonly ILogger<ApproveUser> _logger;
        private readonly IUserService _userService;
        public ApproveUser(ILogger<ApproveUser> log,IUserService userservice)
        {
            _logger = log;
            _userService = userservice;
        }

        [FunctionName("ApproveUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "user/approve/{code}")] HttpRequest req,string code)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await _userService.ApproveUserAsync(code);
            }
            catch (UserNotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }

            return new AcceptedResult();
        }
    }
}

