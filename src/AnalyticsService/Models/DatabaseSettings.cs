namespace AnalyticsService.Models
{
    public interface IDatabaseSettings
    {
        public string UserEventCollection { get; set; }
        public string PostEventCollection { get; set; }
        public string AuthEventCollection { get; set; }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string UserEventCollection { get; set; }
        public string PostEventCollection { get; set; }
        public string AuthEventCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}