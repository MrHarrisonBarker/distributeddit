using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace PostService.Contexts
{
    public class PostContextSeed
    {
        public static async Task SeedAsync(PostContext postContext, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                // INFO: Run this if using a real database. Used to automaticly migrate docker image of sql server db.
                postContext.Database.Migrate();
                //orderContext.Database.EnsureCreated();

                if (!postContext.Posts.Any())
                {
                    postContext.Posts.AddRange(GetPreconfiguredOrders());
                    await postContext.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                if (retryForAvailability < 5)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<PostContextSeed>();
                    log.LogError(exception.Message);
                    await SeedAsync(postContext, loggerFactory, retryForAvailability);
                }

                throw;
            }
        }

        private static IEnumerable<Post> GetPreconfiguredOrders()
        {
            return new List<Post>()
            {
                new Post()
                {
                    Title = "Post title",Body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",Created = DateTime.Now
                },
                new Post()
                {
                    Title = "Another post title",Body = "Fusce vehicula lacus ac odio vestibulum bibendum.",Created = DateTime.Now
                },
            };
        }
    }
}