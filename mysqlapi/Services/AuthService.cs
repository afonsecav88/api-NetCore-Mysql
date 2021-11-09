
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using mysqlapi.Interfaces;
using mysqlapi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace mysqlapi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly StudentDetailContext _context;
        public AuthService(IConfiguration configuration, StudentDetailContext context)
        {
            _configuration = configuration;
            _context = context;
        }
       
        public bool ValidateLogin(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username && x.Password == password);            
           
            return (user == null) ? false : true;                  
        }

        public string GenerateToken(DateTime currentDate, string username, TimeSpan timeValidity)
        {
            var expirationDate = currentDate.Add(timeValidity);
        
            //Configuramos las claims
            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,username),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(currentDate).ToUniversalTime().ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
            new Claim("roles","Cliente"),
            new Claim("roles","Administrador"),
            };

            //Añadimos las credenciales
            var signinKey = _configuration["AuthenticationSettings:SigningKey"];
            var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signinKey)),
                    SecurityAlgorithms.HmacSha256Signature
            );//luego se debe configurar para obtener estos valores, así como el issuer y audience desde el appsetings.json

            //Configuracion del jwt token
            var jwt = new JwtSecurityToken(
                issuer: _configuration["AuthenticationSettings:Issuer"],
                audience: _configuration["AuthenticationSettings:Audience"],
                claims: claims,
                notBefore: currentDate,
                expires: expirationDate,
                signingCredentials: signingCredentials
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return  encodedJwt;
        }    

      
    }
}
