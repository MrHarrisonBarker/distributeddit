using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnalyticsService.Models
{
    public class UserEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Info { get; set; }
        public DateTime Created { get; set; }
        public UserEventAction Action { get; set; }
        public bool Successful { get; set; }
    }

    public enum UserEventAction
    {
        Get,
        Update,
        Create,
        NewPost,
        DestroyPost,
        Destroy
    }
}