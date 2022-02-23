using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class User
    {
        public string? First_name {get; set;}
        public string? Last_name { get; set;}
        public string Email { get; set; }
        public string? Password { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;
        public DateTime Modified { get; set; }=DateTime.Now;

    }
}
