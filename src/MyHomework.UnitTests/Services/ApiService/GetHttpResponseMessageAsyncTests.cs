using AutoFixture;
using FluentAssertions;
using MockHttpClient;
using Moq;
using MyHomework.Constants;
using MyHomework.Models;
using MyHomework.Services;
using MyHomework.UnitTests;

namespace ApiServiceTests
{
    [TestFixture]
    public class GetHttpResponseMessageAsyncTests : TestBase
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;

        [SetUp]
        public void SetUp()
        {
            _mockHttpClientFactory = _mockRepository.Create<IHttpClientFactory>();
        }

        [Test]
        public async Task GivenRequestShouldReturnHttpResponseMessage()
        {
            // Arrange
            var mockResponse = MockHelper.Create<ApiResponse>();
            var mockHttpResponseMessage = new HttpResponseMessage().WithJsonContent(mockResponse);
            var mockHttpClient = new MockHttpClient.MockHttpClient
            {
                BaseAddress = new Uri("http://example.com")
            };

            mockHttpClient
                .When(string.Empty)
                .Then(req => mockHttpResponseMessage);

            _mockHttpClientFactory.Setup(x => x.CreateClient(HttpClientNames.UsersHttpClient)).Returns(mockHttpClient);

            var sut = new ApiService(_mockHttpClientFactory.Object);

            // Act
            var response = await sut.GetHttpResponseMessageAsync(_fixture.Create<CancellationToken>());

            // Assert
            response.Should().Be(mockHttpResponseMessage);
        }
    }
}