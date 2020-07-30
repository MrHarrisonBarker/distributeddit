using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace UserService.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PostId> PostIds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // var splitStringConverter =
            //     new ValueConverter<IList<Guid>, string>(v => string.Join(";", v.Select(x => x.ToString())), v => v.Split(new [] {';'}).Select(Guid.Parse).ToList() );
            // builder.Entity<User>().Property(nameof(User.PostIds)).HasConversion(splitStringConverter);

            // builder.Entity<User>().Property(x => x.PostIds)
            //     .HasConversion(
            //         c => JsonConvert.SerializeObject(c),
            //         c => JsonConvert.DeserializeObject<IList<Guid>>(c)
            //     );

            builder.Entity<User>()
                .HasMany(x => x.PostIds)
                .WithOne(x => x.User);
        }
    }
}