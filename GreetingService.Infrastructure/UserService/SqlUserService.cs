using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure.UserService
{
    public class SqlUserService : IUserService
    {
        private readonly GreetingDbContext _greetingDbContext;
        private readonly ILogger<SqlUserService> _logger;
        public SqlUserService(GreetingDbContext greetingDbContext,ILogger<SqlUserService> logger)
        {           
            _greetingDbContext = greetingDbContext;
            _logger = logger;
        }

        public async Task CreateAsync(User user)
        {
            await _greetingDbContext.Users.AddAsync(user);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string Email)
        {
            var deleteuser=_greetingDbContext.Users.FirstOrDefault(x => x.email == Email);
            _greetingDbContext.Users.Remove(deleteuser);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task<User> GetAsync(string Email)
        {
            var newuser=await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.email == Email);
            return newuser;
        }

        public bool IsValidUser(string username, string Password)
        {
            var user = _greetingDbContext.Users.FirstOrDefault(x => x.email == username);
            if(user != null && user.password==Password)
                return true;

            return false;
        }

        public async Task<IEnumerable<User>> ReadAsync()
        {
            var listofusers = await _greetingDbContext.Users.ToListAsync();
            return listofusers;
        }

        public async Task UpdateAsync(User user)
        {
           var existinguser=await _greetingDbContext.Users.FirstOrDefaultAsync(x =>x.email == user.email);
            if (existinguser == null)
                throw new Exception($"User with email : {user.email} not found");

            existinguser.first_name=user.first_name;
            existinguser.last_name=user.last_name;
            existinguser.password=user.password;

            await _greetingDbContext.SaveChangesAsync();  
        }
    }
}
