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
    public class DeleteUsers_ByEmail
    {
        private readonly ILogger<DeleteUsers_ByEmail> _logger;
        public readonly IUserService _userservice;
        private IAuthHandler Authhandler { get; set; }
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault };
        private Guid new_id;

        public DeleteUsers_ByEmail(ILogger<DeleteUsers_ByEmail> log, IUserService userservice, IAuthHandler _iauthHandler)
        {
            _logger = log;
            _userservice = userservice;
            Authhandler = _iauthHandler;
        }



        [FunctionName("DeleteUsers_ByEmail")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        // [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<User>), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "User/{Email}")] HttpRequest req, string Email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //var body = await req.ReadAsStringAsync();
            ////var users = JsonSerializer.Deserialize<User>(body, _jsonSerializerOptions);

            //var user = JsonConvert.DeserializeObject<User>(body);

            //user.Id = id;


            if (Authhandler.IsAuthorized(req))
            {
                try
                {
                    
                    await _userservice.DeleteAsync(Email);
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
