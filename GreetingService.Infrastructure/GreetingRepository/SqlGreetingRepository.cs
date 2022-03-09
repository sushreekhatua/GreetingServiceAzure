using GreetingService.Core.Entities;
using GreetingService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using GreetingService.Core.Exceptions;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class SqlGreetingRepository : IGreetingRepository
    {
        private readonly GreetingDbContext _greetingDbContext;
        private readonly ILogger<SqlGreetingRepository> _logger;
       

        public SqlGreetingRepository(GreetingDbContext greetingDbContext, ILogger<SqlGreetingRepository> logger)
        {
            _greetingDbContext = greetingDbContext;
            _logger = logger;
        }


        //To read the greetings
        public async Task<IEnumerable<Greeting>> ReadAsync()
        {
            try
            {
                var listofgreetings = await _greetingDbContext.Greetings.ToListAsync();

                return listofgreetings;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        public async Task CreateAsync(Greeting greeting)
        {

            try
            {
                await _greetingDbContext.Greetings.AddAsync(greeting);
                await _greetingDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

        }

        public async Task DeleteAsync(Guid id)
        {
            
            var deletegreeting=  await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == id);

            _greetingDbContext.Greetings.Remove(deletegreeting);

            _greetingDbContext.SaveChanges();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var newidgreeting= await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == id);
            //if(newidgreeting==null)
            //    throw new Exception("Not found");
            
            return newidgreeting;
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            if(!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.From.Equals(from) && x.To.Equals(to));
                return await greetings.ToListAsync();
            }
            else if(!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.From.Equals(from));
                return await greetings.ToListAsync();
            }
            else if (string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.To.Equals(to));
                return await greetings.ToListAsync();
            }
            
                
                return await _greetingDbContext.Greetings.ToListAsync();
            
        }


        public async Task UpdateAsync(Greeting greeting)
        {
            var existingGreeting =await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == greeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;

            await _greetingDbContext.SaveChangesAsync();
        }
    }
}
