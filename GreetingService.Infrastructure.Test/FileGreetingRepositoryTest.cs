using GreetingService.Core.Entities;
using GreetingService.Infrastructure;
using GreetingService.Infrastructure.GreetingRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;


namespace GreetingService.Infrastructure.Test
{
    public class FileGreetingRepositoryTest
    {
        private readonly string filepath;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, };
        public FileGreetingRepository filegreetingrepo_ { get; set; }
        public List<Greeting> newgreetinglists { get; set; }

        //Constructor of FileGreetingRepositoryTest
        public FileGreetingRepositoryTest()
        {
            filepath = $"greeting_filerepo_test_{DateTime.Now.Year}.json";
            filegreetingrepo_ = new FileGreetingRepository(filepath);
            newgreetinglists = new List<Greeting>() 
            {
                new Greeting()
                {
                    Name ="Sadhana",
                    To ="Sweden@gmail.com",
                    From ="India@gmail.com",
                },
                new Greeting()
                {
                    Name ="Sadhana1",
                    To ="Sweden@gmail.com",
                    From ="India@gmail.com",
                },
                new Greeting()
                {
                    Name ="Sadhana2",
                    To ="Sweden2@gmail.com",
                    From ="India@gmail.com",
                },
                new Greeting()
                {
                    Name ="Sadhana3",
                    To ="Sweden3@gmail.com",
                    From ="India@gmail.com",
                }
            };
            File.WriteAllText(filepath, JsonSerializer.Serialize(newgreetinglists, _jsonSerializerOptions));

        }



        [Fact]
        public async Task test_for_all_truegreeting_files()
        {
            var checkfiles1=await filegreetingrepo_.ReadAsync();
            Assert.NotNull(checkfiles1);
            Assert.True(checkfiles1.Count()>1);
            Assert.Equal(newgreetinglists.Count(), checkfiles1.ToList().Count());
        }

        [Fact]
        public async Task get_should_return_correct_greeting()
        {
            var expectedGreeting1 = newgreetinglists[0];
            var actualGreeting1 = await filegreetingrepo_.GetAsync(expectedGreeting1.Id);
            Assert.NotNull(actualGreeting1);
            Assert.Equal(expectedGreeting1.Id, actualGreeting1.Id);

            var expectedGreeting2 = newgreetinglists[1];
            var actualGreeting2 = await filegreetingrepo_.GetAsync(expectedGreeting2.Id);
            Assert.NotNull(actualGreeting2);
            Assert.Equal(expectedGreeting2.Id, actualGreeting2.Id);
        }

        [Fact]
        public async Task post_should_persist_to_file()
        {
            var greetingsBeforeCreate = await filegreetingrepo_.ReadAsync();

            var newGreeting = new Greeting
            {
                From = "post_test@gmail.com",
                To = "post_test@gmail.com",
                Message = "post_test",
            };

            await filegreetingrepo_.CreateAsync(newGreeting);

            var greetingsAfterCreate = await filegreetingrepo_.ReadAsync();

            Assert.Equal(greetingsBeforeCreate.Count() + 1, greetingsAfterCreate.Count());
        }

        [Fact]
        public async Task update_should_persist_to_file()
        {
            var greetings = await filegreetingrepo_.ReadAsync();

            var firstGreeting = greetings.First();
            var firstGreetingMessage = firstGreeting.Message;

            var testMessage = "new updated message";
            firstGreeting.Message = testMessage;

            await filegreetingrepo_.UpdateAsync(firstGreeting);

            var firstGreetingAfterUpdate = await filegreetingrepo_.GetAsync(firstGreeting.Id);
            Assert.Equal(testMessage, firstGreetingAfterUpdate.Message);
        }
    }
}