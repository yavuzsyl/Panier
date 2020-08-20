using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Panier.Contracts.V1.Responses;
using Panier.Domain.Data;
using Panier.Domain.Entities;
using Panier.Options;
using Panier.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Panier.Services.Concrete
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JwtSettings options;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly PanierContext dataContext;

        public IdentityService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager, IOptions<JwtSettings> options,
            TokenValidationParameters tokenValidationParameters, PanierContext dataContext
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.options = options.Value;
            this.tokenValidationParameters = tokenValidationParameters;
            this.dataContext = dataContext;
        }
        //public static int counter = 0; used to add roles to user 
        public async Task<AuthenticationResponse> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthenticationResponse { Errors = new[] { "user doesnt exist" } };

            var userHasValidPassword = await userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
                return new AuthenticationResponse { Errors = new[] { "password is invalid" } };

            //if (counter == 0)
            //{
            //await userManager.AddToRoleAsync(user, "Admin");
            //    counter++;
            //}
            //else
            //await userManager.AddToRoleAsync(user, "Poster");


            return await GetAuthenticationResultAsync(user);
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
                return new AuthenticationResponse { Errors = new[] { "Invalid token" } };

            //token expire time
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationResponse { Errors = new[] { "this token hasnt expired yet" } };

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
                return new AuthenticationResponse { Errors = new[] { "this refresh token doesnt exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationResponse { Errors = new[] { "this refresh token has expired" } };
            if (storedRefreshToken.InValidated)
                return new AuthenticationResponse { Errors = new[] { "this refresh token has been invalidate" } };
            if (storedRefreshToken.Used)
                return new AuthenticationResponse { Errors = new[] { "this refresh token has been used" } };
            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResponse { Errors = new[] { "this refresh doesnt match this JWT" } };

            storedRefreshToken.Used = true;
            dataContext.RefreshTokens.Update(storedRefreshToken);
            await dataContext.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "Id").Value);
            return await GetAuthenticationResultAsync(user);

        }
        //token generate edeck refreshtoken guid olarakauto generate olacak db ye atılacak refreshtokenın jwtId si tokenın payloadundaki jti olacak , yeni token almaya geldiğinde refresh token ile token valid mi ona bakılacak daha sonra refresh token db de var mı diğer kontroller ve refresh tokenın jwtid tokeninki ile aynı mı aynı ise ok yeni token dönecek yeni refresh token ile
        /// <summary>
        /// validates given token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var _tokenValidationParameters = tokenValidationParameters.Clone();
                _tokenValidationParameters.ValidateLifetime = false;//to validate token even it s expired
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }

        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<AuthenticationResponse> RegisterAsync(string email, string password)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return new AuthenticationResponse { Errors = new[] { "user with this email exist" } };

            var newUserId = Guid.NewGuid();
            var newUser = new AppUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = email
            };

            var createdUser = await userManager.CreateAsync(newUser, password);//gonna hash this pass mofo
            if (!createdUser.Succeeded)
                return new AuthenticationResponse { Errors = createdUser.Errors.Select(x => x.Description) };

            //added claim for policy authorization
            //await userManager.AddClaimAsync(newUser, new Claim(type: "tags.view", "true"));

            //token
            return await GetAuthenticationResultAsync(newUser);
        }

        private async Task<AuthenticationResponse> GetAuthenticationResultAsync(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(options.SecretKey);


            var claims = new List<Claim>()
            {
                new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Email),
                new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email),
                new Claim(type: "Id", value: user.Id),
                new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
            };       

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(options.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await dataContext.RefreshTokens.AddAsync(refreshToken);
            await dataContext.SaveChangesAsync();

            return new AuthenticationResponse
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                Success = true,
                Errors = null

            };
        }

    }

}
