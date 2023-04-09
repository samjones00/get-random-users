using System.Text;
using Microsoft.Extensions.Logging;
using MyHomework.Interfaces;
using MyHomework.Models;
using MyHomework.Models.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static MyHomework.Models.AppResponse;

namespace MyHomework.Services
{
    public class UserService : IUserService
    {
        private readonly IApiService _apiService;
        private readonly IFileProvider _dataProvider;
        private readonly ILogger<UserService> _logger;
        private readonly ConfigurationOptions _configurationOptions;

        public UserService(IApiService apiService, IFileProvider dataProvider, ILogger<UserService> logger, ConfigurationOptions configurationOptions)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationOptions = configurationOptions ?? throw new ArgumentNullException(nameof(configurationOptions));
        }

        public async Task GetAndSaveUsers(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting users and returning content responses");
            var userResponses = await GetUsers(cancellationToken);

            _logger.LogInformation("Saving API responses");
            await _dataProvider.WriteAsync(_configurationOptions.ResponsesFilePath, userResponses, cancellationToken);

            _logger.LogInformation("Preparing user results");
            var allResponses = await CombineApiResponses();
            var mappedUsers = MapUsers(allResponses);
            var result = Serialize(mappedUsers);

            _logger.LogInformation("Saving user results", result);
            await _dataProvider.WriteAsync(_configurationOptions.OutputFilePath, result, cancellationToken);
        }

        private async Task<string> GetUsers(CancellationToken cancellationToken)
        {
            var tasks = Enumerable.Range(0, _configurationOptions.RequestCount)
                 .Select(x => _apiService.GetHttpResponseMessageAsync(cancellationToken))
                 .ToList();

            await Task.WhenAll(tasks);
            if (tasks.Any(x => !x.Result.IsSuccessStatusCode))
            {
                _logger.LogError($"{tasks.Count(x => x.Result.IsSuccessStatusCode)} of {tasks.Count} requests successful, see logs for details.");
            }

            var jsonResponses = new StringBuilder();
            foreach (var result in tasks.Select(x => x.Result).Where(x => x.IsSuccessStatusCode))
            {
                jsonResponses.AppendLine(await result.Content.ReadAsStringAsync(cancellationToken));
            }

            return jsonResponses.ToString();
        }

        private async Task<IEnumerable<ApiResponse.User>> CombineApiResponses()
        {
            var savedResponsesJson = await _dataProvider.ReadAsync(_configurationOptions.ResponsesFilePath);

            if (string.IsNullOrWhiteSpace(savedResponsesJson))
            {
                return Enumerable.Empty<ApiResponse.User>();
            }

            return savedResponsesJson
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Take(_configurationOptions.RequestCount)
                .Select(JsonConvert.DeserializeObject<ApiResponse>)
                .SelectMany(x => x.Results);
        }

        private static IEnumerable<User> MapUsers(IEnumerable<ApiResponse.User> source) =>
            source.Select(x => new User(x.Name.Last, x.Name.First, x.Location.City, x.Email, x.DOB.Age));

        private static string Serialize(object source) => JsonConvert.SerializeObject(source, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}