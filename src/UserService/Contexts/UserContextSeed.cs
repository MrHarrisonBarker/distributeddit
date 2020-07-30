using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace UserService.Contexts
{
    public class UserContextSeed
    {
        public static async Task SeedAsync(UserContext userContext, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                // INFO: Run this if using a real database. Used to automaticly migrate docker image of sql server db.
                userContext.Database.Migrate();
                //orderContext.Database.EnsureCreated();

                if (!userContext.Users.Any())
                {
                    userContext.Users.AddRange(GetPreconfiguredOrders());
                    await userContext.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                if (retryForAvailability < 5)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<UserContextSeed>();
                    log.LogError(exception.Message);
                    await SeedAsync(userContext, loggerFactory, retryForAvailability);
                }

                throw;
            }
        }

        private static IEnumerable<BuildingBlocks.Models.User> GetPreconfiguredOrders()
        {
            return new List<BuildingBlocks.Models.User>()
            {
                new BuildingBlocks.Models.User()
                {
                    DisplayName = "Harrison Barker", Email = "harrison@thebarkers.me.uk", Password = "Password",
                    Avatar = "https://pbs.twimg.com/profile_images/1100081397568548866/P00xzKiT_400x400.jpg"
                },
                new BuildingBlocks.Models.User()
                {
                    DisplayName = "COol Dud", Email = "COol@dud.co.uk", Password = "Password",
                    Avatar = "https://pbs.twimg.com/profile_images/1082744382585856001/rH_k3PtQ_400x400.jpg"
                },
            };
        }
    }
}