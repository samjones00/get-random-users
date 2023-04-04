using System.Threading;
using Moq;
using MyHomework.Interfaces;
using MyHomework.Models.Configuration;
using MyHomework.Services;
using MyHomework.UnitTests;

namespace UserServiceTests
{
    public class RunTests : TestBase
    {
        private UserService _sut;
        private Mock<IDataProvider> _mockDataProvider;
        private Mock<IApiService> _mockApiService;
        private ConfigurationOptions _configurationOptions;

        [SetUp]
        public void SetUp()
        {
            _mockDataProvider = _mockRepository.Create<IDataProvider>();
            _mockApiService = _mockRepository.Create<IApiService>();

            _configurationOptions = new ConfigurationOptions
            {
                ApiCallCount = 5,
                DataProviderOutputFileName = "c:\\temp\\MyTest.json",
                DataProviderResponseFileName = "c:\\temp\\MyTest.txt",
                UserServiceBaseUrl = "https://randomuser.me/",
                UserServiceGetEndpoint = "api/"
            };

            _sut = new UserService(_mockDataProvider.Object, _mockApiService.Object, _configurationOptions);
        }

        [Test]
        public async Task GivenSuccessfulApiResponsesWhenExecutingShouldSaveUsers()
        {
            // Arrange
            var jsonContent = "{\"results\":[{\"gender\":\"female\",\"name\":{\"title\":\"Mrs\",\"first\":\"Evie\",\"last\":\"Kumar\"},\"location\":{\"street\":{\"number\":7127,\"name\":\"Bairds Road\"},\"city\":\"New Plymouth\",\"state\":\"Gisborne\",\"country\":\"New Zealand\",\"postcode\":26819,\"coordinates\":{\"latitude\":\"51.0940\",\"longitude\":\"68.2867\"},\"timezone\":{\"offset\":\"+9:30\",\"description\":\"Adelaide, Darwin\"}},\"email\":\"evie.kumar@example.com\",\"login\":{\"uuid\":\"466eb093-2307-4ddb-a927-9b93141b028a\",\"username\":\"heavymouse758\",\"password\":\"beretta\",\"salt\":\"oaD3YT6L\",\"md5\":\"bad1bde1e7ad562a92d367f641abcafa\",\"sha1\":\"6cc352a967c5635c3cd45e875ee69c884e7656af\",\"sha256\":\"7de8a2cf50d242c6be4eeb989c1fefc76882c20e46fb27987e8bdf6a1e777351\"},\"dob\":{\"date\":\"1978-01-02T09:25:22.797Z\",\"age\":45},\"registered\":{\"date\":\"2006-01-23T09:52:49.504Z\",\"age\":17},\"phone\":\"(207)-015-6059\",\"cell\":\"(403)-940-3552\",\"id\":{\"name\":\"\",\"value\":null},\"picture\":{\"large\":\"https://randomuser.me/api/portraits/women/7.jpg\",\"medium\":\"https://randomuser.me/api/portraits/med/women/7.jpg\",\"thumbnail\":\"https://randomuser.me/api/portraits/thumb/women/7.jpg\"},\"nat\":\"NZ\"}],\"info\":{\"seed\":\"e09d354c725abcf7\",\"results\":1,\"page\":1,\"version\":\"1.4\"}}";
            var response = new HttpResponseMessage
            {
                Content = new StringContent(jsonContent)
            };

            var expectedResult = "[{\"last\":\"Kumar\",\"first\":\"Evie\",\"city\":\"New Plymouth\",\"email\":\"evie.kumar@example.com\",\"age\":45}]";

            _mockApiService.Setup(x => x.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(jsonContent);
            _mockDataProvider.Setup(x => x.WriteAsync(_configurationOptions.DataProviderResponseFileName, jsonContent, true, It.IsAny<CancellationToken>()));
            _mockDataProvider.Setup(x => x.ReadAsync(_configurationOptions.DataProviderResponseFileName)).ReturnsAsync(jsonContent);
            _mockDataProvider.Setup(x => x.WriteAsync(_configurationOptions.DataProviderOutputFileName, expectedResult, false, It.IsAny<CancellationToken>()));

            // Act
            await _sut.Run(CancellationToken.None);

            // Assert
        }
    }
}