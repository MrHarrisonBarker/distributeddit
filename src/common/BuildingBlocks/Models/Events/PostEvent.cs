using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnalyticsService.Models
{
    public class PostEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Info { get; set; }
        public DateTime Created { get; set; }
        public PostEventAction Action { get; set; }
        public bool Successful { get; set; }
    }
    
    public enum PostEventAction
    {
        Get,
        Update,
        Create,
        Destroy
    }
}