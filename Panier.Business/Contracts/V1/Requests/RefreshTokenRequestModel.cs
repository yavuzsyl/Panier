using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Panier.Business.Contracts.V1.Requests
{
    public class RefreshTokenRequestModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
