
using Panier.Business.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Services.Abstract
{
    public interface IIdentityService
    {
        Task<AuthenticationResponse> RegisterAsync(string email, string password);
        Task<AuthenticationResponse> LoginAsync(string email, string password);
        Task<AuthenticationResponse> RefreshTokenAsync(string token, string refreshToken);
      
    }
}
