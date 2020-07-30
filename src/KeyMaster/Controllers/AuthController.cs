using System.Threading.Tasks;
using BuildingBlocks.Models;
using KeyMaster.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.Logging;

namespace KeyMaster.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        
        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<AuthToken>> Authenticate([FromBody] Authenticate authenticate)
        {
            var authToken = await _authService.Authenticate(authenticate.Email, authenticate.Password);

            if (authToken == null)
            {
                return BadRequest("No user dud");
            }
            
            
            return authToken;
        }   
    }
}