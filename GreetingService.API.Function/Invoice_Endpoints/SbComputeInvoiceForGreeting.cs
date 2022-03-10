using System;
using System.Linq;
using System.Threading.Tasks;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.Invoice_Endpoints
{
    public class SbComputeInvoiceForGreeting
    {
        private readonly ILogger<SbComputeInvoiceForGreeting> _logger;
        private readonly IInvoiceService _invoiceservice;
        private readonly IGreetingRepository _igreetingrepository;
        private readonly IUserService _iuserservice;


        public SbComputeInvoiceForGreeting(ILogger<SbComputeInvoiceForGreeting> log, IUserService iuserservice, IGreetingRepository igreetingrepository, IInvoiceService invoiceService)
        {
            _logger = log;
            _iuserservice = iuserservice;
            _invoiceservice = invoiceService;
            _igreetingrepository = igreetingrepository;

        }

        [FunctionName("SbComputeInvoiceForGreeting")]
        public async Task Run([ServiceBusTrigger("main", "greeting_compute_billing", Connection = "ServiceBusConnectionString")]Greeting greeting)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {greeting}");
            try
            {
                var newinvoice = await _invoiceservice.GetInvoiceAsync(greeting.Time.Year, greeting.Time.Month, greeting.From);
                if (newinvoice == null)
                {
                    var user1= await _iuserservice.GetAsync(greeting.From);
                    newinvoice = new Invoice()
                    {
                        sender = user1,
                        InvoiceYear = greeting.Time.Year,
                        InvoiceMonth = greeting.Time.Month
                    };
                    await _invoiceservice.CreateOrUpdateInvoiceAsync(newinvoice);


                    newinvoice = await _invoiceservice.GetInvoiceAsync(greeting.Time.Year, greeting.Time.Month, greeting.From);
                    newinvoice.Greetings.Append(greeting);
                    await _invoiceservice.CreateOrUpdateInvoiceAsync(newinvoice);

                }
                else if(!newinvoice.Greetings.Any(x=>x.Id==greeting.Id))
                {
                    newinvoice.Greetings.Append(greeting);
                    await _invoiceservice.CreateOrUpdateInvoiceAsync(newinvoice);
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
