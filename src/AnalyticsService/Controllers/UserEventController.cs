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
    public class UserEventController : ControllerBase
    {
        private readonly IUserEventService _userEventService;
        
        public UserEventController(IUserEventService userEventService)
        {
            _userEventService = userEventService;
        }
        
        [HttpGet]
        public async Task<List<UserEvent>> Get()
        {
            Console.WriteLine("Getting post events");
            return await _userEventService.Get();
        }
    }
}