using MyHomework.Constants;
using MyHomework.Interfaces;
using MyHomework.Models.Configuration;

namespace MyHomework.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigurationOptions _options;

        public ApiService(IHttpClientFactory httpClientFactory, ConfigurationOptions options)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);

            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpClient = httpClientFactory.CreateClient(HttpClientNames.UsersHttpClient);
        }

        public async Task<string> GetAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_options.UserServiceGetEndpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}