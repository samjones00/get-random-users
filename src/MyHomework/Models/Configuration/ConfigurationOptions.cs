using System.ComponentModel.DataAnnotations;

namespace MyHomework.Models.Configuration
{
    public class ConfigurationOptions
    {
        public static string SectionName = "Options";

        public string UserServiceBaseUrl { get; set; }
        public string UserServiceGetEndpoint { get; set; }
        public string DataProviderResponseFileName { get; set; }
        public string DataProviderOutputFileName { get; set; }
        public string LogFileName { get; set; }
        public int ApiCallCount { get; set; }
    }
}