namespace MyHomework.Interfaces
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetAsHttpResponseAsync(CancellationToken cancellationToken);
        Task<string> GetAsStringAsync(CancellationToken cancellationToken);
    }
}