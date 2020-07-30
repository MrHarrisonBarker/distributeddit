using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnalyticsService.Models;
using AnalyticsService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostEventController : ControllerBase
    {
        private readonly IPostEventService _postEventService;
        
        public PostEventController(IPostEventService postEventService)
        {
            _postEventService = postEventService;
        }
        
        [HttpGet]
        public async Task<List<PostEvent>> Get()
        {
            Console.WriteLine("Getting post events");
            return await _postEventService.Get();
        }
    }
}