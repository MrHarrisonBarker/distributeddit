using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnalyticsService.Models;
using BuildingBlocks.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PostService.Contexts;

namespace PostService
{
    public interface IPostService
    {
        Task<IList<Post>> Get();
        Task<IList<Post>> GetUserPosts(Guid userId);
        Task<Post> GetById(Guid id);
        Task<Post> Create(Post post);
        Task<Post> Update(Post post);
        Task<bool> Destroy(Guid id);
        Task<bool> DestroyUserPosts(User user);
    }

    public class PostService : IPostService
    {
        private readonly PostContext _postContext;
        private readonly IRequestClient<Post> _request;
        private readonly IRequestClient<DestroyPostRequest> _destroyPostRequest;
        private readonly IPublishEndpoint _publish;

        public PostService(PostContext postContext, IRequestClient<Post> request, IPublishEndpoint publish,
            IRequestClient<DestroyPostRequest> destroyPostRequest)
        {
            _postContext = postContext;
            _request = request;
            _publish = publish;
            _destroyPostRequest = destroyPostRequest;
        }

        public async Task<IList<Post>> Get()
        {
            var posts = await _postContext.Posts.ToListAsync();
            await _publish.Publish<PostEvent>(new PostEvent()
            {
                Info = "Get all",
                Created = DateTime.Now,
                Action = PostEventAction.Get,
                Successful = posts != null
            });
            return posts;
        }

        public async Task<Post> GetById(Guid id)
        {
            var post = await _postContext.Posts.FirstOrDefaultAsync(x => x.Id == id);

            await _publish.Publish<PostEvent>(new PostEvent()
            {
                Info = $"Get by id -> {id}",
                Created = DateTime.Now,
                Action = PostEventAction.Get,
                Successful = post != null
            });
            return post;
        }

        public async Task<Post> Create(Post post)
        {
            post.Created = DateTime.Now;
            await _postContext.Posts.AddAsync(post);
            await _postContext.SaveChangesAsync();

            var response = await _request.GetResponse<AddPostResponse>(post);

            if (response.Message.Added)
            {
                await _publish.Publish<PostEvent>(new PostEvent()
                {
                    Info = $"Create for user -> {post.Owner}",
                    Created = DateTime.Now,
                    Action = PostEventAction.Create,
                    Successful = true
                });
                Console.WriteLine("Post has been added to user");
                return post;
            }

            await _publish.Publish<PostEvent>(new PostEvent()
            {
                Info = $"Create for user -> {post.Owner} -> user not found",
                Created = DateTime.Now,
                Action = PostEventAction.Create,
                Successful = false
            });

            Console.WriteLine("Post was not added to user now rolling back");

            _postContext.Posts.Remove(post);
            await _postContext.SaveChangesAsync();
            return null;
        }

        public Task<Post> Update(Post post)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Destroy(Guid id)
        {
            var post = await _postContext.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                return true;
            }

            var response = await _destroyPostRequest.GetResponse<DestroyPostResponse>(new DestroyPostRequest()
            {
                Id = post.Id
            });

            if (response.Message.Destroyed)
            {
                _postContext.Posts.Remove(post);
                await _postContext.SaveChangesAsync();
                return true;
            }
            
            return false;
        }

        public async Task<bool> DestroyUserPosts(User user)
        {
            Console.WriteLine($"Getting posts for user -> {user.Id}");

            var posts = await _postContext.Posts.Where(x => x.Owner == user.Id).ToListAsync();

            Console.WriteLine($"Got post for user -> {posts.Count}");

            _postContext.RemoveRange(posts);

            try
            {
                await _postContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IList<Post>> GetUserPosts(Guid userId)
        {
            Console.WriteLine($"Getting posts for user {userId}");
            return await _postContext.Posts.Where(x => x.Owner == userId).ToListAsync();
        }
    }
}