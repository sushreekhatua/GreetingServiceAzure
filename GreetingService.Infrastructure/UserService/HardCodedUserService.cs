using GreetingService.Core;
using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.UserService
{
    public class HardCodedUserService : IUserService
    {
        private static IDictionary<string, string> _users = new Dictionary<string, string>()
        {
            { "sadhana","summer2022" },
            { "maria","winter2022" },
        };

        public async Task CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

      

        public async Task DeleteAsync(string email)
        {
            throw new NotImplementedException();
        }

       

        public async Task<User> GetAsync(string email)
        {
            throw new NotImplementedException();
        }

        public bool IsValidUser(string username, string password)
        {
            if (!_users.TryGetValue(username, out var storedPassword))              //user does not exist
                return false;

            if (!storedPassword.Equals(password))
                return false;

            return true;
        }

        public async Task<IEnumerable<User>> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
