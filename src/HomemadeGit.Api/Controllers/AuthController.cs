using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomemadeGit.Core.DTOs;
using HomemadeGit.Core.Interfaces;

namespace HomemadeGit.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState });
            }
            try
            {
                var user = await _authService.RegisterAsync(request);

                return Ok(user);
            }
            catch (Exception ex) {
                return BadRequest(new { errors = ex.Message });
            }
        }
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState });
            }

            try
            {
                var user = await _authService.LoginAsync(request);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new {errors = ex.Message});
            }
        }
    }
}
