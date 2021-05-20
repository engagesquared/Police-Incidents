namespace PoliceIncidents.Functions
{
    public static class WebConfig
    {
        public static readonly string DbConnectionString = System.Environment.GetEnvironmentVariable($"{Prefix}ConnectionString");
        private const string Prefix = "PoliceIncidents";
    }
}
