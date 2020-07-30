using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BuildingBlocks.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }

        public List<PostId> PostIds { get; set; }
    }

    public class PostId
    {
        [JsonIgnore] public User User { get; set; }
        public Guid Post { get; set; }
        [JsonIgnore] public Guid Id { get; set; }
    }

    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public IList<PostId> PostIds { get; set; }
    }

    public class UserPostsViewModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public IList<PostViewModel> Posts { get; set; }
    }

    public class DestroyUserPostsRequest
    {
        public User User { get; set; }
    }

    public class DestroyUserPostsResponse
    {
        public bool Destroyed { get; set; }
    }

    public class GetUserPostsRequest
    {
        public User User { get; set; }
    }
    
    public class GetUserPostsResponse
    {
        public IList<Post> Posts { get; set; }
    }
}