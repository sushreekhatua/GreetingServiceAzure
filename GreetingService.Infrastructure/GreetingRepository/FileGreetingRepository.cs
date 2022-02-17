using GreetingService.Core;
using GreetingService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using GreetingService.Core.Interfaces;

namespace GreetingService.Infrastructure.GreetingRepository
{
    public class FileGreetingRepository : IGreetingRepository
    {
        private readonly string _filepath;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public FileGreetingRepository(string filepath)
        {
            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, "[]");
            }
            
            _filepath = filepath;
            

        }

        public async Task CreateAsync(Greeting greeting)
        {
            var content =File.ReadAllText(_filepath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            
            if (greetings.Any(x => x.Id == greeting.Id))
                throw new Exception($"Greeting with id: {greeting.Id} already exists");

            greetings.Add(greeting);

            File.WriteAllText(_filepath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
            
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var content = File.ReadAllText(_filepath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings?.FirstOrDefault(x => x.Id == id);
        }

        //Get all greetings
        public async Task<IEnumerable<Greeting>> CreateAsync()
        {
            var content = File.ReadAllText(_filepath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            return greetings;
        }

        public async  Task UpdateAsync(Greeting greeting)
        {
            var content = File.ReadAllText(_filepath);
            var greetings = JsonSerializer.Deserialize<IList<Greeting>>(content);
            var existingGreeting = greetings.FirstOrDefault(x => x.Id == greeting.Id);

            if (existingGreeting == null)
                throw new Exception($"Greeting with id: {greeting.Id} not found");

            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;
            existingGreeting.Message = greeting.Message;

            File.WriteAllText(_filepath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }
        public async Task DeleteAsync(Guid id)
        {
            var content=File.ReadAllText(_filepath);
            var greetings=JsonSerializer.Deserialize<IList<Greeting>>(content);
            var greetingpresent=greetings.FirstOrDefault(a=>a.Id==id);

            if (greetingpresent == null)
                throw new Exception($"Greeting with id: {id} not found");
            else
            {
                
                greetings.Remove(greetingpresent);
            }
            File.WriteAllText(_filepath, JsonSerializer.Serialize(greetings, _jsonSerializerOptions));
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {

            var greetings = await CreateAsync();

            if (!string.IsNullOrWhiteSpace(from))
                greetings = greetings.Where(x => x.From.Equals(from, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(to))
                greetings = greetings.Where(x => x.To.Equals(to, StringComparison.OrdinalIgnoreCase));

            return greetings;


            
                
            

        }
    }
}
