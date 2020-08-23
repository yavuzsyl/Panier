using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panier.Business.Extensions
{
    public static class AuthExtension
    {
        /// <summary>
        /// Get user id from tokens payload / Kullanıcı id bilgisini token payloadundan getir 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
                return string.Empty;

            return httpContext.User.Claims.Single(x => x.Type == "Id").Value;
        }
    }
}
