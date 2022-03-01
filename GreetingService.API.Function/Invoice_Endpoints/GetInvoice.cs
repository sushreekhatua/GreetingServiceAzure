using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core.Helpers;
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
    public class GetInvoice
    {
        private readonly ILogger<GetInvoices> _logger;
        private IAuthHandler _authhandler { get; set; }
        private IInvoiceService _invoiceService { get; set; }

        public GetInvoice(ILogger<GetInvoices> log, IAuthHandler authHandler, IInvoiceService invoiceservice)
        {
            _logger = log;
            _authhandler = authHandler;
            _invoiceService = invoiceservice;

        }

        [FunctionName("GetInvoice")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Invoice" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Accepted, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]



        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoice/{year}/{month}/{email}")] HttpRequest req, int year, int month,string email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!_authhandler.IsAuthorized(req))
                return new UnauthorizedResult();

            if (!ValidationEmail.Emailvalidation(email))
                return new BadRequestObjectResult($"{email} is not a valid email address");

            try
            {
                var invoices = await _invoiceService.GetInvoiceAsync(year, month,email);
                return new OkObjectResult(invoices);
            }
            catch
            {
                return new NotFoundResult();
            }
            
            
        }
    }

}