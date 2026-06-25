using Fundo.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("/auth")]
    [AllowAnonymous]
    public sealed class AuthenticationController : Controller
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenticationController(IJwtTokenGenerator jwtTokenGenerator)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("token")]
        public Task<IActionResult> GenerateToken([FromBody] GenerateTokenRequest request)
        {
            var token = _jwtTokenGenerator.GenerateToken(request.Username, request.Roles ?? new List<string>());
            return Task.FromResult<IActionResult>(Ok(new { accessToken = token }));
        }

        public sealed record GenerateTokenRequest(string Username, List<string> Roles = null);
    }
}
