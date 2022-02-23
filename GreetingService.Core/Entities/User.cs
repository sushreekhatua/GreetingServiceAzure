using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class User
    {
        public string? first_name {get; set;}
        public string? last_name { get; set;}
        public string? email { get; set; }
        public string? password { get; set; }
        public DateTime created { get; set; }= DateTime.Now;
        public DateTime modified { get; set; }=DateTime.Now;

    }
}
