namespace MyHomework.Models
{
    public class ApiResponse
    {
        public IEnumerable<User> Results { get; set; } = new List<User>();
        public record User(Name Name, Location Location, DOB DOB, string Email);
        public record DOB(int Age);
        public record Location(string City, string Last);
        public record Name(string First, string Last);
    }
}