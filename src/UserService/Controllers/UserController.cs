using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IList<UserViewModel>> GetAll()
        {
            return await _userService.Get();
        }

        [HttpGet]
        public async Task<UserPostsViewModel> Get(Guid id)
        {
            return await _userService.GetById(id);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BuildingBlocks.Models.User user)
        {
            if (await _userService.Create(user))
            {
                return Ok("Created");
            }

            return new BadRequestResult();
        }
        
        [HttpPut]
        public async Task<UserViewModel> Update([FromBody] User user)
        {
            return await _userService.Update(user);
        }
        
        [HttpDelete]
        public async Task<bool> Destroy(Guid id)
        {
            return await _userService.Destroy(id);
        }
        
        
    }
}