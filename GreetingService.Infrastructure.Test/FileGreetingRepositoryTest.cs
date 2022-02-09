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
            var checkfiles1=filegreetingrepo_.Create();
            Assert.NotNull(checkfiles1);
            Assert.True(checkfiles1.Count()>1);
            Assert.Equal(newgreetinglists.Count(), checkfiles1.ToList().Count());
        }
    }
}