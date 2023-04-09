using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHomework.Constants;
using MyHomework.Handlers;
using MyHomework.Interfaces;
using MyHomework.Models.Configuration;
using MyHomework.Services;
using Polly;

namespace MyHomework
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection(ConfigurationOptions.SectionName).Get<ConfigurationOptions>();

            services.AddHttpClient(config);

            services.AddSingleton<LoggingHandler>();
            services.AddSingleton(_ => config);
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IFileProvider, SystemIOFileProvider>();

            return services;
        }

        public static ServiceProvider AddLogging(this ServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var config = serviceProvider.GetService<ConfigurationOptions>();

            ArgumentNullException.ThrowIfNull(config?.LogFilePath);
            loggerFactory.AddFile(@config.LogFilePath);

            return serviceProvider;
        }

        private static IServiceCollection AddHttpClient(this IServiceCollection services, ConfigurationOptions config)
        {
            var throttlingPolicy = Policy.BulkheadAsync<HttpResponseMessage>(config.MaxParallelization, int.MaxValue);

            services
                .AddHttpClient(HttpClientNames.UsersHttpClient, x => { x.BaseAddress = new Uri(config.UserServiceUrl); })
                .AddPolicyHandler(throttlingPolicy)
                .AddHttpMessageHandler<LoggingHandler>();

            return services;
        }
    }
}