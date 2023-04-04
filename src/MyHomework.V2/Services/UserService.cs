using Microsoft.Extensions.Logging;
using MyHomework.Interfaces;
using MyHomework.Models;
using MyHomework.Models.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyHomework.Services
{
    public class UserService : IUserService
    {
        private readonly IApiService _apiService;
        private readonly IDataProvider _dataProvider;
        private readonly ILogger<UserService> _logger;
        private readonly ConfigurationOptions _configurationOptions;

        public UserService(IApiService apiService, IDataProvider dataProvider, ILogger<UserService> logger, ConfigurationOptions configurationOptions)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationOptions = configurationOptions ?? throw new ArgumentNullException(nameof(configurationOptions));
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Requesting users");
            await SendRequests(cancellationToken);

            _logger.LogInformation("Preparing user results");
            var users = await CombineResponses();
            var mappedUsers = users.Select(x => new AppResponse.User(x.Name.Last, x.Name.First, x.Location.City, x.Email, x.DOB.Age));
            var result = JsonConvert.SerializeObject(mappedUsers, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            _logger.LogInformation("Writing user results", result);
            _dataProvider.WriteAsync(_configurationOptions.OutputFileName, result, append: false, cancellationToken);
        }

        private async Task SendRequests(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _configurationOptions.ApiCallCount; i++)
            {
                var response = await _apiService.GetAsync(cancellationToken);
                _dataProvider.WriteAsync(_configurationOptions.ResponsesFileName, response, append: true, cancellationToken);
            }
        }

        private async Task<IEnumerable<ApiResponse.User>> CombineResponses()
        {
            var json = await _dataProvider.ReadAsync(_configurationOptions.ResponsesFileName);
            var responses = json.Split(Environment.NewLine).Take(_configurationOptions.ApiCallCount);

            return responses.SelectMany(x => JsonConvert.DeserializeObject<ApiResponse>(x).Results);
        }
    }
}