using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GreetingService.Infrastructure.UserService
{
    public class AppSettingsUserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppSettingsUserService> _logger;

        //private readonly Dictionary<string, string> _userlogin;

        //public AppSettingsUserService(Dictionary<string, string> userlogin)
        //{
        //    _userlogin = userlogin;

        //}

        //public bool IsValidUser(string username1, string password)
        //{
        //    if (!_userlogin.TryGetValue(username1, out var storedPassword))              //user does not exist
        //        return false;

        //    if (!storedPassword.Equals(password))
        //        return false;

        //    return true;
        //}

        public AppSettingsUserService(IConfiguration configuration,ILogger<AppSettingsUserService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

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
            var entries = _configuration.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
            if (entries.TryGetValue(username, out var storedPassword))
            {
                if (storedPassword == password)
                {
                    _logger.LogInformation("Valid credentials for {username}", username);
                    return true;
                }
            }

            _logger.LogWarning("Invalid credentials for {username}", username);
            return false;
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
