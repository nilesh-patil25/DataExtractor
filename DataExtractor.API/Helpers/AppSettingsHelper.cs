namespace DataExtractor.API.Helpers
{
    public static class AppSettingsHelper
    {
        private static IConfiguration _configuration;
        public static void AppSettingConfigure(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetValue(string key) => _configuration.GetSection(key).Value;
    }
}
