using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class SqlInvoiceService : IInvoiceService
    {
        private readonly GreetingDbContext _greetingDbContext;
        //private readonly ILogger<SqlInvoiceService> _logger;

        public SqlInvoiceService(GreetingDbContext greetingDbContext)
        {
            _greetingDbContext = greetingDbContext;
        }
        public async Task CreateOrUpdateInvoiceAsync(Invoice invoice)
        {
            if (! await _greetingDbContext.Invoices.AnyAsync(x => x.sender.Email == invoice.sender.Email && x.InvoiceMonth==invoice.InvoiceMonth && x.InvoiceYear==invoice.InvoiceYear))
            {
                await _greetingDbContext.Invoices.AddAsync(invoice);
                await _greetingDbContext.SaveChangesAsync();
            }
            else
            {
                var existinginvoice = await _greetingDbContext.Invoices.FirstOrDefaultAsync(x => x.sender.Email == invoice.sender.Email && x.InvoiceMonth == invoice.InvoiceMonth && x.InvoiceYear == invoice.InvoiceYear);
                if (existinginvoice == null)
                    throw new Exception($"User with email : {invoice.sender.Email} not found");

                existinginvoice.Greetings = invoice.Greetings;
                existinginvoice.Totalcost= invoice.Totalcost;
                

                await _greetingDbContext.SaveChangesAsync();
            }
            
           
        }

        public async Task<Invoice> GetInvoiceAsync(int year, int month, string email)
        {
            //Only one invoice per user and month should exist
            //Relations are not included by default, here we use .Include() to explicitly state that the query result should include the relations for Greetings and Sender
            var invoice1 = await _greetingDbContext.Invoices.Include(x => x.Greetings)
                                                          .Include(x => x.sender)
                                                          .FirstOrDefaultAsync(x => x.InvoiceYear == year && x.InvoiceMonth == month && x.sender.Email.Equals(email));
            return invoice1;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(int year, int month)
        {
            var invoices = await _greetingDbContext.Invoices.Include(x => x.Greetings)
                                                            .Include(x => x.sender)
                                                            .Where(x => x.InvoiceYear == year && x.InvoiceMonth == month).ToListAsync();
            return invoices;
        }
    }
}
