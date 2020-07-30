using System;
using AnalyticsService.Models;
using MongoDB.Driver;

namespace AnalyticsService.Contexts
{
    public interface IAnalyticsContext
    {
        public IMongoCollection<UserEvent> UserEvents { get; set; }
        public IMongoCollection<PostEvent> PostEvents { get; set; }
        public IMongoCollection<AuthEvent> AuthEvents { get; set; }
    }

    public class AnalyticsContext : IAnalyticsContext
    {
        public IMongoCollection<UserEvent> UserEvents { get; set; }
        public IMongoCollection<PostEvent> PostEvents { get; set; }
        public IMongoCollection<AuthEvent> AuthEvents { get; set; }

        public AnalyticsContext(IDatabaseSettings settings)
        {
            Console.WriteLine("Connecting to db");
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            UserEvents = database.GetCollection<UserEvent>(settings.UserEventCollection);
            PostEvents = database.GetCollection<PostEvent>(settings.PostEventCollection);
            AuthEvents = database.GetCollection<AuthEvent>(settings.AuthEventCollection);
            
            AnalyticsContextSeed.SeedEventData(PostEvents,UserEvents,AuthEvents);
        }
    }
}