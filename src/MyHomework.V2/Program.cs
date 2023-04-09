using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHomework.Interfaces;

/// <summary>
/// We have provided you the code below for a proof of concept (PoC) console application that satisfies the following requirements:
/// - Reads random users from an API endpoint 5 times. 
/// - All responses from the API should be written to a file.
/// - All successful responses should be represented as a list of users with the following properties
///     - last name
///     - first name
///     - age
///     - city
///     - email
///   and be written to file as valid JSON.
/// 
/// There are now new requirements for this application, and we should like for you to update this console 
/// application to incorporate the following new requirements while satisfing the original requirements:
/// - Update this code so it follows .Net best practices and principles. The code should be extensible, reusable, and easy to test using Unit Tests.
/// - Add Unit tests.
/// - Make the the output file names configurable.
/// - Make the number of API calls configurable instead of 5.
/// - Add logging
/// </summary>

namespace MyHomework
{
    internal class Program
    {
        private static IUserService _userService;
        private static ILogger<Program> _logger;
        private static readonly CancellationTokenSource cancellationTokenSource = new();

        static async Task Main(string[] args)
        {
            BindServices();

            try
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    _logger.LogWarning("Cancel event triggered.");
                    cancellationTokenSource.Cancel();
                    eventArgs.Cancel = true;
                };

                await _userService.GetAndSaveUsers(cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Shutting down...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogError(ex.Message, ex.InnerException);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }

        private static void BindServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddServices(configuration)
                .BuildServiceProvider()
                .AddLogging();

            _userService = serviceProvider.GetRequiredService<IUserService>();
            _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        }
    }
}