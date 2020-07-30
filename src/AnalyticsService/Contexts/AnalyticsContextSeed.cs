using System;
using System.Collections.Generic;
using AnalyticsService.Models;
using MongoDB.Driver;

namespace AnalyticsService.Contexts
{
    public class AnalyticsContextSeed
    {
        public static void SeedEventData(IMongoCollection<PostEvent> postCollection,IMongoCollection<UserEvent> userCollection,IMongoCollection<AuthEvent> authCollection)
        {
            bool existPost  = postCollection.Find(p => true).Any();
            if (!existPost)
            {
                postCollection.InsertManyAsync(PreDefinedPostEvents());
            }
            
            bool existUser  = userCollection.Find(p => true).Any();
            if (!existUser)
            {
                userCollection.InsertManyAsync(PreDefinedUserEvents());
            }
            
            bool existAuth  = authCollection.Find(p => true).Any();
            if (!existAuth)
            {
                authCollection.InsertManyAsync(PreDefinedAuthEvents());
            }
        }

        private static IEnumerable<PostEvent> PreDefinedPostEvents()
        {
            return new List<PostEvent>()
            {
                new PostEvent()
                {
                    Info = "PreDefined Create", Action = PostEventAction.Create, Created = DateTime.Now,
                    Successful = true
                },
                new PostEvent()
                {
                    Info = "PreDefined Destroy", Action = PostEventAction.Destroy, Created = DateTime.Now,
                    Successful = true
                }
            };
        }
        
        private static IEnumerable<UserEvent> PreDefinedUserEvents()
        {
            return new List<UserEvent>()
            {
                new UserEvent()
                {
                    Info = "PreDefined Create", Action = UserEventAction.Create, Created = DateTime.Now,
                    Successful = true
                },
                new UserEvent()
                {
                    Info = "PreDefined Destroy", Action = UserEventAction.Destroy, Created = DateTime.Now,
                    Successful = true
                }
            };
        }
        
        private static IEnumerable<AuthEvent> PreDefinedAuthEvents()
        {
            return new List<AuthEvent>()
            {
                new AuthEvent()
                {
                    Info = "PreDefined auth", Action = AuthEventAction.Auth, Created = DateTime.Now,
                    Successful = true
                }
            };
        }
    }
}