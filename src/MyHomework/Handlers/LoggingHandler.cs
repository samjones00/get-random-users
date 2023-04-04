using Microsoft.Extensions.Logging;

namespace MyHomework.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                _logger.LogInformation($"Request {request}:");
                _logger.LogInformation(await request.Content.ReadAsStringAsync());
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation(response.ToString());
            if (response.Content != null)
            {
                _logger.LogInformation("Response:");
                _logger.LogInformation(await response.Content.ReadAsStringAsync());
            }

            return response;
        }
    }
}