using System.Net;
using FluentAssertions;
using MockHttpClient;
using Moq;
using MyHomework.Constants;
using MyHomework.Models;
using MyHomework.Models.Configuration;
using MyHomework.Services;
using MyHomework.UnitTests;

namespace ApiServiceTests
{
    [TestFixture]
    public class GetAsyncTests : TestBase
    {
        private ApiService _sut;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private ConfigurationOptions _configurationOptions;

        [SetUp]
        public void SetUp()
        {
            _mockHttpClientFactory = _mockRepository.Create<IHttpClientFactory>();

            _configurationOptions = MockHelper.Create<ConfigurationOptions>(x =>
            {
                x.UserServiceBaseUrl = "http://domain.com";
                x.UserServiceGetEndpoint = "/api";
            });
        }

        [Test]
        public async Task GivenUnsuccessfulResponseShouldThrowHttpRequestException()
        {
            // Arrange
            var mockHttpClient = new MockHttpClient.MockHttpClient();
            mockHttpClient.BaseAddress = new Uri(_configurationOptions.UserServiceBaseUrl);
            mockHttpClient.When(_configurationOptions.UserServiceGetEndpoint).Then(HttpStatusCode.NotFound);

            _mockHttpClientFactory.Setup(x => x.CreateClient(HttpClientNames.UsersHttpClient)).Returns(mockHttpClient);

            _sut = new ApiService(_mockHttpClientFactory.Object, _configurationOptions);

            // Act
            var act = () => _sut.GetAsHttpResponseAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
        }

        [Test]
        public async Task GivenUnsuccessfulResponseShouldReturnJson()
        {
            // Arrange
            var mockResponse = MockHelper.Create<ApiResponse>();

            var mockHttpClient = new MockHttpClient.MockHttpClient
            {
                BaseAddress = new Uri(_configurationOptions.UserServiceBaseUrl)
            };

            mockHttpClient
                .When(_configurationOptions.UserServiceGetEndpoint)
                .Then(req => new HttpResponseMessage()
                .WithJsonContent(mockResponse));

            _mockHttpClientFactory.Setup(x => x.CreateClient(HttpClientNames.UsersHttpClient)).Returns(mockHttpClient);

            _sut = new ApiService(_mockHttpClientFactory.Object, _configurationOptions);

            // Act
            var response = await _sut.GetAsHttpResponseAsync(CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
        }
    }
}