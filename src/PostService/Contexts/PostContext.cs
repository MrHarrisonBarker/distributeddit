using BuildingBlocks.Models;
using Microsoft.EntityFrameworkCore;

namespace PostService.Contexts
{
    public class PostContext : DbContext
    {
        public PostContext(DbContextOptions<PostContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}