namespace MyHomework.Models
{
    public class AppResponse
    {
        public record User(string Last, string First, string City, string Email, int Age);
    }
}