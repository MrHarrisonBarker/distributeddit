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
    public class AuthEventController : ControllerBase
    {
        private readonly IAuthEventService _authEventService;
        
        public AuthEventController(IAuthEventService authEventService)
        {
            _authEventService = authEventService;
        }
        
        [HttpGet]
        public async Task<List<AuthEvent>> Get()
        {
            Console.WriteLine("Getting post events");
            return await _authEventService.Get();
        }
    }
}