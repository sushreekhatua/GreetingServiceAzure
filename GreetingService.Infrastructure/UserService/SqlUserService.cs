using GreetingService.Core.Entities;
using GreetingService.Core.Exceptions;
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
            var existinguser = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email && x.Approvalstatus != ApprovalStatus.Approved);
            if (await _greetingDbContext.Users.AnyAsync(x => x.Email == user.Email && x.Approvalstatus == ApprovalStatus.Approved))
            {
                return;
            }
            else if (existinguser != null)
            {
                _greetingDbContext.Users.Remove(existinguser);
            }

            user.Created = DateTime.Now;
            user.Modified = DateTime.Now;
            user.Approvalstatus = ApprovalStatus.Pending;
            user.ApprovalStatusNote = "Awaiting approval from administrator";
            await _greetingDbContext.Users.AddAsync(user);
            await _greetingDbContext.SaveChangesAsync();

        }

        public async Task DeleteAsync(string Email)

        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(Email));
            if (user == null)
            {
                _logger.LogWarning("Delete user failed, user with email {email} not found", Email);
                throw new UserNotFoundException($"User {Email} not found");
            }

            _greetingDbContext.Users.Remove(user);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task<User> GetAsync(string Email)
        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals(Email));
            if (user == null)
                return null;

            return user;
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

        private async Task<User> GetUserForApprovalAsync(string approvalCode)
        {
            var user = await _greetingDbContext.Users.FirstOrDefaultAsync(x => x.Approvalstatus == ApprovalStatus.Pending && x.ApprovalCode.Equals(approvalCode) && x.ApprovalExpiry > DateTime.Now);
            if (user == null)
                throw new UserNotFoundException($"User with approval code: {approvalCode} not found");

            return user;
        }

        public async Task ApproveUserAsync(string approvalcode)
        {
            var needapprovaluser= await GetUserForApprovalAsync(approvalcode);
            needapprovaluser.Approvalstatus=ApprovalStatus.Approved;
            needapprovaluser.ApprovalStatusNote=$"Approved by an administrator at {DateTime.Now:O}";
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task RejectUserAsync(string approvalcode)
        {
            var needapprovaluser = await GetUserForApprovalAsync(approvalcode);
            needapprovaluser.Approvalstatus = ApprovalStatus.Rejected;
            needapprovaluser.ApprovalStatusNote = $"Rejected by an administrator at {DateTime.Now:O}";
            await _greetingDbContext.SaveChangesAsync();
        }

    }
}
