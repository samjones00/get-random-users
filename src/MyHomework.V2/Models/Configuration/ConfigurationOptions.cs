namespace MyHomework.Models.Configuration
{
    public class ConfigurationOptions
    {
        public const string SectionName = "Options";

        public string LogFilePath { get; set; }
        public int MaxParallelization { get; set; }
        public string OutputFilePath { get; set; }
        public int RequestCount { get; set; }
        public string ResponsesFilePath { get; set; }
        public string UserServiceUrl { get; set; }
    }
}