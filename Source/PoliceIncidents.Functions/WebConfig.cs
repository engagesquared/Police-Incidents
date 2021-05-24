namespace PoliceIncidents.Functions
{
    public static class WebConfig
    {
        public static readonly string DbConnectionString = System.Environment.GetEnvironmentVariable($"{Prefix}ConnectionString");
        public static readonly string BotBaseUrl = System.Environment.GetEnvironmentVariable($"{Prefix}BotBaseUrl");
        private const string Prefix = "PoliceIncidents";
    }
}
