namespace MyHomework.Interfaces
{
    public interface IApiService
    {
        Task<HttpResponseMessage> GetHttpResponseMessageAsync(CancellationToken cancellationToken);
    }
}