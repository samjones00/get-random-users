namespace MyHomework.Interfaces
{
    public interface IApiService
    {
        Task<string> GetAsync(CancellationToken cancellationToken);
    }
}