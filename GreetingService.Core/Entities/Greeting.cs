namespace GreetingService.Core.Entities
{
    public class Greeting
    {
        public string Message { get; set; } = "Hello Everyone";
        public string? To { get; set; }
        public string? From { get; set; }
        public DateTime Time { get; set; }=DateTime.Now;
        public string Name { get; set; } = "Sadhana";
        public Guid Id { get; set; }= Guid.NewGuid();
        
    }
}
