using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task Run(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Requesting users");
            var json = await SendRequests(cancellationToken);
            await _dataProvider.WriteAsync(_configurationOptions.ResponsesFileName, json, append: false, cancellationToken);

            _logger.LogInformation("Preparing user results");
            var allUsers = await CombineResponses();
            var mappedUsers = allUsers.Select(x => new AppResponse.User(x.Name.Last, x.Name.First, x.Location.City, x.Email, x.DOB.Age));
            var result = Serialize(mappedUsers);

            _logger.LogInformation("Saving user results", result);
            await _dataProvider.WriteAsync(_configurationOptions.OutputFileName, result, append: false, cancellationToken);
        }

        //private async Task<string> SendRequests(CancellationToken cancellationToken)
        //{
        //    var tasks = Enumerable.Range(0, _configurationOptions.ApiCallCount)
        //        .Select(x => { return _apiService.GetAsHttpResponseAsync(cancellationToken); })
        //        .ToList();

        //    await Task.WhenAll(tasks);

        //    if (!tasks.Any(x => x.Result.IsSuccessStatusCode))
        //    {
        //        _logger.LogError($"{tasks.Count(x => x.Result.IsSuccessStatusCode)} of {tasks.Count()} requests successful, see logs for details.");
        //    }


        //    var jsonResponses = new StringBuilder();

        //    foreach (var item in tasks.Where(x => x.Result.IsSuccessStatusCode))
        //    {
        //        jsonResponses.AppendLine(await item.Result.Content.ReadAsStringAsync(cancellationToken));
        //    }

        //    return jsonResponses.ToString();
        //}


        private async Task<string> SendRequests(CancellationToken cancellationToken)
        {
            var tasks = Enumerable.Range(0, _configurationOptions.ApiCallCount)
                 .Select(x => _apiService.GetAsHttpResponseAsync(cancellationToken))
                 .ToList();

            var responses = await Task.WhenAll(tasks);

            if (responses.Any(x => !x.IsSuccessStatusCode))
            {
                _logger.LogError($"{tasks.Count(x => x.Result.IsSuccessStatusCode)} of {tasks.Count()} requests successful, see logs for details.");
            }

            var jsonResponses = new StringBuilder();

            foreach (var response in responses.Where(x => x.IsSuccessStatusCode))
            {
                jsonResponses.AppendLine(await response.Content.ReadAsStringAsync(cancellationToken));
            }

            return jsonResponses.ToString();
        }

        private async Task<IEnumerable<ApiResponse.User>> CombineResponses()
        {
            var savedResponsesJson = await _dataProvider.ReadAsync(_configurationOptions.ResponsesFileName);

            var responses = savedResponsesJson
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Take(_configurationOptions.ApiCallCount);

            return responses.SelectMany(x => JsonConvert.DeserializeObject<ApiResponse>(x).Results);
        }

        private static string Serialize(object source) => JsonConvert.SerializeObject(source, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}