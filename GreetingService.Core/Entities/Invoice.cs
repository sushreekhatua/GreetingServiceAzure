using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class Invoice
    {
        public int? Id { get; set; }
        public User? sender { get; set; }
        public IEnumerable<Greeting>? Greetings { get; set; }
        public int InvoiceYear { get; set; }
        public int InvoiceMonth { get; set; }
        public decimal CostperGreeting { get; set; } = 20;   
        //public decimal Totalcost { get; set; }
        private decimal _totalcost;
        public decimal Totalcost
        {
            get
            {
                if (Greetings == null)
                    return 0;
                return Greetings.Count() * CostperGreeting;
            }
            set
            {
                _totalcost= value;
            }
        }
        public string? Currency { get; set; } = "kr";

    }
}
