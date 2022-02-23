using GreetingService.API.Function.Authentication;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.API.Function.User_Endpoints
{
    public class GetUsers_ByEmail
    {
        private readonly ILogger<GetUsers_ByEmail> _logger;
        public readonly IUserService _userservice;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };
        private Guid new_id;

        public GetUsers_ByEmail(ILogger<GetUsers_ByEmail> log, IUserService userservice, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _userservice = userservice;
            Authhandler = _iauthHandler;
        }



        [FunctionName("GetUsers_By_Email")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        // [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<User>), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/{email}")] HttpRequest req, string email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //var body = await req.ReadAsStringAsync();
            ////var greetings = JsonSerializer.Deserialize<Greeting>(body, _jsonSerializerOptions);

            //var greetings = JsonConvert.DeserializeObject<Greeting>(body);

            //greetings.Id = id;

            if (Authhandler.IsAuthorized(req))
            {
                try
                {
                    
                    var user = await _userservice.GetAsync(email);
                    return new OkObjectResult(user);
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
