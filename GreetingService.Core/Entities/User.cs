using GreetingService.Core.Exceptions;
using GreetingService.Core.Helpers;
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
        //public string Email { get; set; }
        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                if (!ValidationEmail.Emailvalidation(value))
                    throw new InvalidEmailException($"{value} is not a valid email");

                _email = value;
            }
        }
        public string? Password { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;
        public DateTime Modified { get; set; }=DateTime.Now;
        public ApprovalStatus? Approvalstatus{ get; set; }
        public string? ApprovalStatusNote { get; set; } = "This is not approved by Admin";
        public string? ApprovalCode { get; set; }
        public DateTime ApprovalExpiry { get; set; } = DateTime.Now.AddDays(2);


    }

    public enum ApprovalStatus
    {
        Approved=0,
        Rejected=1,
        Pending=2
    }
}
