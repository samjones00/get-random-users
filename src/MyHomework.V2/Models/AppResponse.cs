namespace MyHomework.Models
{
    public class AppResponse
    {
        public record Response(string ResultJson, int RequestCount, int SuccessCount);
        public record User(string Last, string First, string City, string Email, int Age);
    }
}