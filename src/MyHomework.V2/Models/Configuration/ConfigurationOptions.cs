namespace MyHomework.Models.Configuration
{
    public class ConfigurationOptions
    {
        public static string SectionName = "Options";

        public string UserServiceBaseUrl { get; set; }
        public string UserServiceGetEndpoint { get; set; }
        public string ResponsesFileName { get; set; }
        public string OutputFileName { get; set; }
        public string LogFileName { get; set; }
        public int ApiCallCount { get; set; }
    }
}