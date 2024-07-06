using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services;

namespace Service.Services
{
    public class JwtService : IJwtService, IScopedDependency
    {
        private readonly SignInManager<User> _signInManager;
        private readonly SiteSettings _siteSettings;

        public JwtService(IOptionsSnapshot<SiteSettings> settings, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _siteSettings = settings.Value;
        }

        public async Task<AccessToken> GenerateAsync(User user)
        {
            var claims = await _getClaimsAsync(user);
            var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.EncryptKey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionKey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _siteSettings.JwtSettings.Issuer,
                Audience = _siteSettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims),
                EncryptingCredentials = encryptingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);
            // var jwt = tokenHandler.WriteToken(securityToken);

            return new AccessToken(securityToken);
        }
        private async Task<IEnumerable<Claim>> _getClaimsAsync(User user)
        {
            var result = await _signInManager.ClaimsFactory.CreateAsync(user);
            // add custom claims
            var list = new List<Claim>(result.Claims) { new Claim(ClaimTypes.MobilePhone, user.Mobile) };
            return list;
        }
    }
}
