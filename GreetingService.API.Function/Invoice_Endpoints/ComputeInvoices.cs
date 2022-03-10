using System;
using System.Linq;
using System.Threading.Tasks;
using GreetingService.Core.Entities;
using GreetingService.Core.Enums;
using GreetingService.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.Invoice_Endpoints
{
    public class ComputeInvoices
    {

        private readonly IInvoiceService _invoiceservice;
        private readonly IGreetingRepository _igreetingrepository;
        private readonly IUserService _iuserservice;

        public ComputeInvoices(IUserService iuserservice,IGreetingRepository igreetingrepository,IInvoiceService invoiceService)
        {
            _iuserservice = iuserservice;
            _invoiceservice = invoiceService;
            _igreetingrepository = igreetingrepository;
        }

        [FunctionName("ComputeInvoices")]
        public async Task Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
           
            // First I will collect all greetings
            var allgreetings = await _igreetingrepository.ReadAsync();

            // Let's filter out all greetings with groupby from,year and month
            var newgreetingsgroupby = allgreetings.GroupBy(c => new
            {
                c.From,
                c.Time.Year,
                c.Time.Month
            });
            foreach(var g in newgreetingsgroupby)
            {
                var users1 = await _iuserservice.GetAsync(g.Key.From);
                var invoicess = new Invoice() 
                {
                    sender = users1,
                    Greetings=g,
                    InvoiceYear=g.Key.Year,
                    InvoiceMonth=g.Key.Month

                };

                

                await _invoiceservice.CreateOrUpdateInvoiceAsync(invoicess);
            }
        }
    }
}
