using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnalyticsService.Models;
using AutoMapper;
using BuildingBlocks.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserService.Contexts;
using UserService.Migrations;

namespace UserService
{
    public interface IUserService
    {
        Task<IList<UserViewModel>> Get();
        Task<User> GetByEmail(string email);
        Task<UserPostsViewModel> GetById(Guid id);
        Task<bool> Create(User user);
        Task<UserViewModel> Update(User user);
        Task<bool> Destroy(Guid id);
        Task<bool> AddPost(Post post);
        Task<bool> DestroyPost(Guid postId);
    }

    public class UserService : IUserService
    {
        private readonly UserContext _userContext;
        private readonly IRequestClient<DestroyUserPostsRequest> _destroyUserPostsRequest;
        private readonly IRequestClient<GetUserPostsRequest> _getUserPostRequest;
        private readonly IPublishEndpoint _publish;
        private readonly IMapper _mapper;

        public UserService(UserContext userContext, IPublishEndpoint publish,
            IRequestClient<DestroyUserPostsRequest> destroyUserPostsRequest,
            IRequestClient<GetUserPostsRequest> getUserPostRequest, IMapper mapper)
        {
            _userContext = userContext;
            _publish = publish;
            _destroyUserPostsRequest = destroyUserPostsRequest;
            _getUserPostRequest = getUserPostRequest;
            _mapper = mapper;
        }

        public async Task<IList<UserViewModel>> Get()
        {
            var user = await _userContext.Users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Avatar = x.Avatar,
                Email = x.Email,
                PostIds = x.PostIds
            }).ToListAsync();

            await _publish.Publish<UserEvent>(new UserEvent()
            {
                Info = $"Get all",
                Created = DateTime.Now,
                Action = UserEventAction.Get,
                Successful = user != null
            });
            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            await _publish.Publish<UserEvent>(new UserEvent()
            {
                Info = $"Get user by email -> {user.Email}",
                Created = DateTime.Now,
                Action = UserEventAction.Get,
                Successful = user != null
            });
            return user;
        }

        public async Task<UserPostsViewModel> GetById(Guid id)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                await _publish.Publish<UserEvent>(new UserEvent()
                {
                    Info = $"Get user by id -> {id} -> user not found",
                    Created = DateTime.Now,
                    Action = UserEventAction.Get,
                    Successful = false
                });
                return null;
            }

            var posts = await GetUserPosts(user);

            // Console.WriteLine($"Got posts for user {posts[0].Id}");

            var userWithPosts = new UserPostsViewModel()
            {
                Id = user.Id,
                Avatar = user.Avatar,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Posts = posts.Message.Posts.Select(x => _mapper.Map<PostViewModel>(x)).ToList()
            };

            await _publish.Publish<UserEvent>(new UserEvent()
            {
                Info = $"Get user by id -> {id}",
                Created = DateTime.Now,
                Action = UserEventAction.Get,
                Successful = true
            });
            return userWithPosts;
        }

        private async Task<Response<GetUserPostsResponse>> GetUserPosts(User user)
        {
            return await _getUserPostRequest.GetResponse<GetUserPostsResponse>(new GetUserPostsRequest()
                {User = user});
        }

        [HttpPost("create")]
        public async Task<bool> Create(User user)
        {
            var existingUser = await _userContext.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (existingUser != null)
            {
                await _publish.Publish<UserEvent>(new UserEvent()
                {
                    Info = $"Create user -> {user.Id} -> user already exists",
                    Created = DateTime.Now,
                    Action = UserEventAction.Create,
                    Successful = false
                });
                throw new Exception("User already exists in db");
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>(
                new OptionsWrapper<PasswordHasherOptions>(
                    new PasswordHasherOptions
                    {
                        CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
                    })
            );

            user.Password = hasher.HashPassword(user, user.Password);

            await _userContext.Users.AddAsync(user);
            try
            {
                await _publish.Publish<UserEvent>(new UserEvent()
                {
                    Info = $"Create user -> {user.Id}",
                    Created = DateTime.Now,
                    Action = UserEventAction.Create,
                    Successful = true
                });
                await _userContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw new Exception();
            }
        }

        public Task<UserViewModel> Update(User user)
        {
            // TODO: Implement update user
            return null;
        }

        public async Task<bool> Destroy(Guid id)
        {
            var user = await _userContext.Users.Include(x => x.PostIds).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return false;
            }

            var response =
                await _destroyUserPostsRequest.GetResponse<DestroyUserPostsResponse>(new DestroyUserPostsRequest()
                    {User = user});

            if (!response.Message.Destroyed) return false;

            var postIds = await _userContext.PostIds.Where(x => user.PostIds.Contains(x)).ToListAsync();

            Console.WriteLine($"Got post ids {postIds.Count}");


            _userContext.PostIds.RemoveRange(postIds);
            _userContext.Users.Remove(user);

            await _userContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddPost(Post post)
        {
            var user = await _userContext.Users.Include(x => x.PostIds).FirstOrDefaultAsync(x => x.Id == post.Owner);

            if (user == null)
            {
                Console.WriteLine("Not Found");
                return false;
            }

            Console.WriteLine($"got user from the db {user.DisplayName} now adding post to user");

            PostId postId = new PostId();
            postId.Post = post.Id;
            postId.User = user;

            Console.WriteLine("Created the post id");

            await _userContext.PostIds.AddAsync(postId);

            Console.WriteLine($"post id list {JsonConvert.SerializeObject(user.PostIds)}");
            await _userContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DestroyPost(Guid postId)
        {
            var post = await _userContext.PostIds.FirstOrDefaultAsync(x => x.Post == postId);

            if (post == null)
            {
                return true;
            }

            try
            {
                _userContext.PostIds.Remove(post);
                await _userContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                Console.WriteLine("Error removing post id");
                return false;
            }
        }
    }
}