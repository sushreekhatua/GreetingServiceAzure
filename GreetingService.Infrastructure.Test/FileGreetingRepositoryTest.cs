using GreetingService.Core.Entities;
using GreetingService.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
                    To ="Sweden",
                    From ="India",
                },
                new Greeting()
                {
                    Name ="Sadhana1",
                    To ="Sweden1",
                    From ="India1",
                },
                new Greeting()
                {
                    Name ="Sadhana2",
                    To ="Sweden2",
                    From ="India2",
                },
                new Greeting()
                {
                    Name ="Sadhana3",
                    To ="Sweden3",
                    From ="India3",
                }
            };
            File.WriteAllText(filepath, JsonSerializer.Serialize(newgreetinglists, _jsonSerializerOptions));

        }



        [Fact]
        public void test_for_all_truegreeting_files()
        {
            #var checkfiles1=filegreetingrepo_.Create();
            Assert.NotNull(checkfiles1);
            Assert.True(checkfiles1.Count()>1);
            Assert.Equal(newgreetinglists.Count(), checkfiles1.ToList().Count());
        }

        [Fact]
        public void get_should_return_correct_greeting()
        {
            var expectedGreeting1 = newgreetinglists[0];
            var actualGreeting1 = filegreetingrepo_.Get(expectedGreeting1.Id);
            Assert.NotNull(actualGreeting1);
            Assert.Equal(expectedGreeting1.Id, actualGreeting1.Id);

            var expectedGreeting2 = newgreetinglists[1];
            var actualGreeting2 = filegreetingrepo_.Get(expectedGreeting2.Id);
            Assert.NotNull(actualGreeting2);
            Assert.Equal(expectedGreeting2.Id, actualGreeting2.Id);
        }

        [Fact]
        public void post_should_persist_to_file()
        {
            var greetingsBeforeCreate = filegreetingrepo_.Create();

            var newGreeting = new Greeting
            {
                From = "post_test",
                To = "post_test",
                Message = "post_test",
            };

            filegreetingrepo_.Create(newGreeting);

            var greetingsAfterCreate = filegreetingrepo_.Create();

            Assert.Equal(greetingsBeforeCreate.Count() + 1, greetingsAfterCreate.Count());
        }

        [Fact]
        public void update_should_persist_to_file()
        {
            var greetings = filegreetingrepo_.Create();

            var firstGreeting = greetings.First();
            var firstGreetingMessage = firstGreeting.Message;

            var testMessage = "new updated message";
            firstGreeting.Message = testMessage;

            filegreetingrepo_.Update(firstGreeting);

            var firstGreetingAfterUpdate = filegreetingrepo_.Get(firstGreeting.Id);
            Assert.Equal(testMessage, firstGreetingAfterUpdate.Message);
        }
    }
}