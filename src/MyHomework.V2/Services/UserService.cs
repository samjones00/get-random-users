using System.Text;
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
            var sendStatus = await SendRequests(cancellationToken);

            _logger.LogInformation("Preparing user results");
            var allUsers = await CombineResponses();
            var mappedUsers = allUsers.Select(x => new AppResponse.User(x.Name.Last, x.Name.First, x.Location.City, x.Email, x.DOB.Age));
            var result = JsonConvert.SerializeObject(mappedUsers, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            _logger.LogInformation("Writing user results", result);
            _dataProvider.WriteAsync(_configurationOptions.OutputFileName, result, append: false, cancellationToken);
        }

        private async Task<bool> SendRequests(CancellationToken cancellationToken)
        {
            var jsonResponses = new StringBuilder();
            var tasks = Enumerable.Range(0, _configurationOptions.ApiCallCount)
                .Select(x => { return _apiService.GetAsHttpResponseAsync(cancellationToken); })
                .ToList();

            var aggregateTask = await Task.WhenAll(tasks);

            if (!tasks.Any(x => x.Result.IsSuccessStatusCode))
            {
                _logger.LogError($"{tasks.Count(x => x.Result.IsSuccessStatusCode)} of {tasks.Count()} requests successful, see logs for details.");
            }

            foreach (var item in tasks.Where(x => x.Result.IsSuccessStatusCode))
            {
                jsonResponses.AppendLine(await item.Result.Content.ReadAsStringAsync());
            }

            await Task.WhenAll(tasks).ContinueWith(x =>
            {
                _dataProvider.WriteAsync(_configurationOptions.ResponsesFileName, jsonResponses.ToString(), append: false, cancellationToken);
            }, cancellationToken);

            return true;
        }

        private async Task<IEnumerable<ApiResponse.User>> CombineResponses()
        {
            var savedResponsesJson = await _dataProvider.ReadAsync(_configurationOptions.ResponsesFileName);

            var responses = savedResponsesJson
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Take(_configurationOptions.ApiCallCount);

            return responses.SelectMany(x => JsonConvert.DeserializeObject<ApiResponse>(x).Results);
        }
    }
}