using GreetingService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Interfaces
{
    public interface IUserService
    {
        public bool IsValidUser(string username, string password);

        public Task<User> GetAsync(string email);
        public Task<IEnumerable<User>> ReadAsync();
        public Task CreateAsync(User user);
        public Task UpdateAsync(User user);
        public Task DeleteAsync(string email);
        //public Task<IEnumerable<User>> GetAsync(string from, string to);

    }
}
