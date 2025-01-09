using Entity;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace Middlewares
{
    public class GenerateToken
    {
        private readonly string _SecretKey;
        private readonly string _issuer;
        private readonly string _audience;
        public GenerateToken(string secretKey, string issuer, string audience)
        {
            _SecretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public Token GenerateJwtToken(Usuario user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Nombre),
                 new Claim(ClaimTypes.Role, user.Rol),                
                 new Claim("IdCompania", user.IdCompania),
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Puedes agregar más claims según tus necesidades
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Ajusta la expiración según tus necesidades
                signingCredentials: credentials
            );
            Token token1 = new Token();
            token1.token = new JwtSecurityTokenHandler().WriteToken(token);
            return token1;
        }
    }
}
