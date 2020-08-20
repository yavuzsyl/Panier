using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Panier.Contracts;
using Panier.Contracts.V1.Requests;
using Panier.Contracts.V1.Responses;
using Panier.Services.Abstract;

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
        // [ApiKeyAuth]//herkeşler register olamaz only those who got the apikeys are allowed
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

        /// <summary>
        /// the client needs to store the expiry date in the local storage and on every request you need to check if it’s in the past. If it is then you use a middleware to call the refresh endpoint and get a new set of keys.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
