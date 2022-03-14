using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.Core.Exceptions;
using GreetingService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function.User_Endpoints
{
    public class RejectUser
    {
        private readonly ILogger<RejectUser> _logger;
        private readonly IUserService userservice;

        public RejectUser(ILogger<RejectUser> log, IUserService userService)
        {
            _logger = log;
            userservice = userService;
        }

        [FunctionName("RejectUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "User/reject/{code}")] HttpRequest req,string code)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                await userservice.RejectUserAsync(code);
            }
            catch (UserNotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }

            return new AcceptedResult();

        }
    }
}

