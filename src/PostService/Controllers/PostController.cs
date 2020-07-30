using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;

        public PostController(ILogger<PostController> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }

        [HttpGet("all")]
        public async Task<IList<Post>> Get()
        {
            return await _postService.Get();
        }

        [HttpGet]
        public async Task<Post> Get(Guid id)
        {
            return await _postService.GetById(id);
        }

        [HttpPost]
        public async Task<Post> Create([FromBody] Post post)
        {
            return await _postService.Create(post);
        }
        
        [HttpGet("user")]
        public async Task<IList<Post>> GetUserPosts(Guid userId)
        {
            return await _postService.GetUserPosts(userId);
        }
        
        [HttpPut]
        public async Task<Post> Update([FromBody] Post post)
        {
            return await _postService.Update(post);
        }
        
        [HttpDelete]
        public async Task<bool> Destroy(Guid id)
        {
            return await _postService.Destroy(id);
        }
    }
}
