
using GreetingService.Core.Exceptions;
using GreetingService.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class Greeting
    {
        public string Message { get; set; } = "Hello Everyone";
        private string _to;
        public string To
        {
            get
            {
                return _to;
            }

            set
            {
                if (!ValidationEmail.Emailvalidation(value))
                    throw new InvalidEmailException($"{value} is not a valid email");

                _to = value;
            }
        }
        private string _from;
        public string From
        {
            get
            {
                return _from;
            }

            set
            {
                if (!ValidationEmail.Emailvalidation(value))
                    throw new InvalidEmailException($"{value} is not a valid email");

                _from = value;
            }
        }
        public DateTime Time { get; set; }=DateTime.Now;
        public string Name { get; set; } = "Sadhana";
        public Guid Id { get; set; }= Guid.NewGuid();
        
    }
}
