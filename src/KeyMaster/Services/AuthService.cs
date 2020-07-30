using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AnalyticsService.Models;
using BuildingBlocks.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KeyMaster.Services
{
    public interface IAuthService
    {
        Task<AuthToken> Authenticate(string email, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IRequestClient<AuthContract> _request;
        private readonly IPublishEndpoint _publish;

        public AuthService(IRequestClient<AuthContract> request, IOptions<AppSettings> appSettings,IPublishEndpoint publish)
        {
            _request = request;
            _appSettings = appSettings.Value;
            _publish = publish;
        }

        public async Task<AuthToken> Authenticate(string email, string password)
        {
            var response = await _request.GetResponse<User>(new {Email = email});
            var user = response.Message;
            
            Console.WriteLine($"Got user for auth {user.DisplayName}");

            PasswordHasher<User> hasher = new PasswordHasher<User>(
                new OptionsWrapper<PasswordHasherOptions>(
                    new PasswordHasherOptions()
                    {
                        CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
                    })
            );

            // if (user == null)
            // {
            //     Console.WriteLine("User not found");
            //     throw new InvalidOperationException("not found");
            //     return null;
            // }
            
            if (hasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
            {
                await _publish.Publish<AuthEvent>(new AuthEvent()
                {
                    Info = $"Auth -> password incorrect",
                    Created = DateTime.Now,
                    Action = AuthEventAction.Auth,
                    Successful = false
                });
                Console.WriteLine("Password incorrect");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var authToken = new AuthToken()
            {
                Token = tokenHandler.WriteToken(token),
                Expires = (int) TimeSpan.FromDays(7).TotalSeconds
            };
            
            await _publish.Publish<AuthEvent>(new AuthEvent()
            {
                Info = $"Auth -> {user.Id}",
                Created = DateTime.Now,
                Action = AuthEventAction.Auth,
                Successful = true
            });

            return authToken;
        }
    }
}