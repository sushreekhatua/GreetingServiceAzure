using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
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

namespace GreetingService.API.Function.Invoice_Endpoints
{
    public class GetInvoices
    {
        private readonly ILogger<GetInvoices> _logger;
        private IAuthHandler _authhandler { get; set; }
        private IInvoiceService _invoiceService { get; set; }

        public GetInvoices(ILogger<GetInvoices> log,IAuthHandler authHandler,IInvoiceService invoiceservice)
        {
            _logger = log;
            _authhandler = authHandler;
            _invoiceService = invoiceservice;
            
        }

        [FunctionName("GetInvoices")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Invoice" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Accepted, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        
        
        
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoice/{year}/{month}")] HttpRequest req,int year,int month)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if(_authhandler.IsAuthorized(req))
            {
                try
                {
                    var manyinvoices = await _invoiceService.GetInvoicesAsync(year, month);
                    return new OkObjectResult(manyinvoices);
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

