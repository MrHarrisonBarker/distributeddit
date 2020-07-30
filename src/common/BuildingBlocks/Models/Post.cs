using System;
using System.Collections.Generic;

namespace BuildingBlocks.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        
        public Guid Owner { get; set; } 
    }

    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
    }

    public class AddPostResponse
    {
        public bool Added { get; set; }
    }

    public class DestroyPostRequest
    {
        public Guid Id { get; set; }
    }
    
    public class DestroyPostResponse
    {
        public bool Destroyed { get; set; }
    }
}