using AuthOnion.Api.Application.DTOs;
using AuthOnion.Api.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthOnion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
                await _auth.RegisterAsync(req);
                return Ok(new { message = "Registered" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var res = await _auth.LoginAsync(req);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            var user = User.Identity?.Name ?? "unknown";
            return Ok(new { message = "You are authenticated", user = User.Claims.Select(c => new { c.Type, c.Value }) });
        }
    }
}
