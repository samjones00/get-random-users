using Microsoft.Extensions.Logging.Abstractions;
using MockHttpClient;
using Moq;
using MyHomework.Interfaces;
using MyHomework.Models.Configuration;
using MyHomework.Services;
using MyHomework.UnitTests;

namespace UserServiceTests
{
    public class GetAndSaveUsersTests : TestBase
    {
        private UserService _sut;
        private NullLogger<UserService> _logger;
        private Mock<IFileProvider> _mockDataProvider;
        private Mock<IApiService> _mockApiService;
        private ConfigurationOptions _configurationOptions;

        [SetUp]
        public void SetUp()
        {
            _logger = new NullLogger<UserService>();
            _mockDataProvider = _mockRepository.Create<IFileProvider>();
            _mockApiService = _mockRepository.Create<IApiService>();

            _configurationOptions = MockHelper.Create<ConfigurationOptions>(x =>
            {
                x.RequestCount = 2;
            });

            _sut = new UserService(_mockApiService.Object, _mockDataProvider.Object, _logger, _configurationOptions);
        }

        [Test]
        public async Task GivenSuccessfulApiResponsesWhenExecutingShouldSaveUsers()
        {
            // Arrange
            var mockFirstHttpResponseJson = "{\"results\":[{\"gender\":\"male\",\"name\":{\"title\":\"Mr\",\"first\":\"Felipe\",\"last\":\"Lozano\"},\"location\":{\"street\":{\"number\":3507,\"name\":\"Calle de Alcalá\"},\"city\":\"Vitoria\",\"state\":\"La Rioja\",\"country\":\"Spain\",\"postcode\":31137,\"coordinates\":{\"latitude\":\"-36.5862\",\"longitude\":\"-93.1039\"},\"timezone\":{\"offset\":\"-8:00\",\"description\":\"Pacific Time (US & Canada)\"}},\"email\":\"felipe.lozano@example.com\",\"login\":{\"uuid\":\"57254fc3-f8f5-4286-a962-f2279159f0b4\",\"username\":\"happydog950\",\"password\":\"mason\",\"salt\":\"7G9wjFAi\",\"md5\":\"24e32ad3eef9d36cf4874aaad8f3c431\",\"sha1\":\"4d1dc5cf5b724ceb77fa603abb017f0802efc6bd\",\"sha256\":\"b0e4fc94bbd8d3aca00c8604c415d6a758246238069a50aac08dac1905bdf679\"},\"dob\":{\"date\":\"1953-09-25T22:03:59.717Z\",\"age\":69},\"registered\":{\"date\":\"2005-09-15T01:13:09.618Z\",\"age\":17},\"phone\":\"951-369-072\",\"cell\":\"644-417-576\",\"id\":{\"name\":\"DNI\",\"value\":\"61302978-Q\"},\"picture\":{\"large\":\"https://randomuser.me/api/portraits/men/49.jpg\",\"medium\":\"https://randomuser.me/api/portraits/med/men/49.jpg\",\"thumbnail\":\"https://randomuser.me/api/portraits/thumb/men/49.jpg\"},\"nat\":\"ES\"}],\"info\":{\"seed\":\"e22dff2d9ddff44d\",\"results\":1,\"page\":1,\"version\":\"1.4\"}}";
            var mockSecondHttpResponseJson = "{\"results\":[{\"gender\":\"male\",\"name\":{\"title\":\"Mr\",\"first\":\"Isaac\",\"last\":\"Harper\"},\"location\":{\"street\":{\"number\":2653,\"name\":\"Henry Street\"},\"city\":\"Roscommon\",\"state\":\"Galway\",\"country\":\"Ireland\",\"postcode\":39462,\"coordinates\":{\"latitude\":\"-0.4817\",\"longitude\":\"-53.2572\"},\"timezone\":{\"offset\":\"-11:00\",\"description\":\"Midway Island, Samoa\"}},\"email\":\"isaac.harper@example.com\",\"login\":{\"uuid\":\"75044c55-7eda-4216-995f-54d56927260a\",\"username\":\"greenzebra419\",\"password\":\"method\",\"salt\":\"E31Pjgz2\",\"md5\":\"547eeaec014e74c041925d94bf902f79\",\"sha1\":\"1cec577bbb608f44ad34abfccdc5659d3bc57257\",\"sha256\":\"4d6347bb67455ca270261e9a11a886a41215b65200ecc12ec1cac0b394dad2b6\"},\"dob\":{\"date\":\"1962-02-05T21:12:22.351Z\",\"age\":61},\"registered\":{\"date\":\"2019-03-31T16:58:00.913Z\",\"age\":4},\"phone\":\"031-614-1306\",\"cell\":\"081-240-3647\",\"id\":{\"name\":\"PPS\",\"value\":\"6780941T\"},\"picture\":{\"large\":\"https://randomuser.me/api/portraits/men/34.jpg\",\"medium\":\"https://randomuser.me/api/portraits/med/men/34.jpg\",\"thumbnail\":\"https://randomuser.me/api/portraits/thumb/men/34.jpg\"},\"nat\":\"IE\"}],\"info\":{\"seed\":\"6b2cb5b4d352daf1\",\"results\":1,\"page\":1,\"version\":\"1.4\"}}";
            var mockSavedResponsesJson = mockFirstHttpResponseJson + Environment.NewLine + mockSecondHttpResponseJson + Environment.NewLine;

            var expectedResultJson = "[" +
                "{\"last\":\"Lozano\",\"first\":\"Felipe\",\"city\":\"Vitoria\",\"email\":\"felipe.lozano@example.com\",\"age\":69}," +
                "{\"last\":\"Harper\",\"first\":\"Isaac\",\"city\":\"Roscommon\",\"email\":\"isaac.harper@example.com\",\"age\":61}" +
                "]";

            _mockApiService.SetupSequence(x => x.GetHttpResponseMessageAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage().WithStringContent(mockFirstHttpResponseJson))
                .ReturnsAsync(new HttpResponseMessage().WithStringContent(mockSecondHttpResponseJson));

            _mockDataProvider.Setup(x => x.WriteAsync(_configurationOptions.ResponsesFilePath, mockSavedResponsesJson, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockDataProvider.Setup(x => x.ReadAsync(_configurationOptions.ResponsesFilePath)).ReturnsAsync(mockSavedResponsesJson);
            _mockDataProvider.Setup(x => x.WriteAsync(_configurationOptions.OutputFilePath, expectedResultJson, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _sut.GetAndSaveUsers(It.IsAny<CancellationToken>());

            // Assert
            _mockApiService.Verify(x => x.GetHttpResponseMessageAsync(It.IsAny<CancellationToken>()), Times.Exactly(_configurationOptions.RequestCount));
            _mockDataProvider.Verify(x => x.WriteAsync(_configurationOptions.ResponsesFilePath, mockSavedResponsesJson, It.IsAny<CancellationToken>()), Times.Once);
            _mockDataProvider.Verify(x => x.ReadAsync(_configurationOptions.ResponsesFilePath), Times.Once);
            _mockDataProvider.Verify(x => x.WriteAsync(_configurationOptions.OutputFilePath, expectedResultJson, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}