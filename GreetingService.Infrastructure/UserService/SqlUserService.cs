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
            var deleteuser=_greetingDbContext.Users.FirstOrDefault(x => x.Email == Email);
            _greetingDbContext.Users.Remove(deleteuser);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task<User> GetAsync(string Email)
        {
            var newuser=await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email == Email);
            return newuser;
        }

        public bool IsValidUser(string username, string Password)
        {
            var user = _greetingDbContext.Users.FirstOrDefault(x => x.Email == username);
            if(user != null && user.Password==Password)
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
           var existinguser=await _greetingDbContext.Users.FirstOrDefaultAsync(x =>x.Email == user.Email);
            if (existinguser == null)
                throw new Exception($"User with email : {user.Email} not found");

            existinguser.First_name=user.First_name;
            existinguser.Last_name=user.Last_name;
            existinguser.Password=user.Password;

            await _greetingDbContext.SaveChangesAsync();  
        }
    }
}
