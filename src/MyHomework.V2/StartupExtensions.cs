using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHomework.Constants;
using MyHomework.Handlers;
using MyHomework.Interfaces;
using MyHomework.Models.Configuration;
using MyHomework.Services;

namespace MyHomework
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection(ConfigurationOptions.SectionName).Get<ConfigurationOptions>();

            services
                .AddHttpClient(HttpClientNames.UsersHttpClient, x => { x.BaseAddress = new Uri(config.UserServiceBaseUrl); })
                .AddHttpMessageHandler<LoggingHandler>();

            services.AddSingleton<LoggingHandler>();
            services.AddSingleton(_ => config);
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFileProvider, SystemIOFileProvider>();

            return services;
        }

        public static ServiceProvider AddLogging(this ServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var config = serviceProvider.GetService<ConfigurationOptions>();

            ArgumentNullException.ThrowIfNull(config?.LogFileName);
            loggerFactory.AddFile(@config.LogFileName);

            return serviceProvider;
        }
    }
}