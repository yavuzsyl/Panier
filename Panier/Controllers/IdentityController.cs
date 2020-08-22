
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Panier.Business.Contracts;
using Panier.Business.Contracts.V1.Requests;
using Panier.Business.Services.Abstract;

namespace Panier.Controllers
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestModel request)
        {
            var registrationResponse = await identityService.RegisterAsync(request.Email, request.Password);
            if (!registrationResponse.Success)
                return BadRequest(registrationResponse);

            return Ok(registrationResponse);
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {

            var loginResponse = await identityService.LoginAsync(request.Email, request.Password);
            if (!loginResponse.Success)
                return BadRequest(loginResponse);

            return Ok(loginResponse);
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh(RefreshTokenRequestModel request)
        {
            var refreshResponse = await identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (!refreshResponse.Success)
                return BadRequest(refreshResponse);

            return Ok(refreshResponse);
        }

    }
}
