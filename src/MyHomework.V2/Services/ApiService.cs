using MyHomework.Constants;
using MyHomework.Interfaces;

namespace MyHomework.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);
            _httpClient = httpClientFactory.CreateClient(HttpClientNames.UsersHttpClient);
        }

        public async Task<HttpResponseMessage> GetHttpResponseMessageAsync(CancellationToken cancellationToken) =>
            await _httpClient.GetAsync(string.Empty, cancellationToken);
    }
}