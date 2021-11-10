
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mysqlapi.Interfaces;
using System;

namespace mysqlapi.Controllers.v1
{

    [ApiController]
    [Route("api/v{version:apiVersion}/login")]
    [ApiVersion("1.0")]   
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Loguea al usuario y genera un token de autenticación
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost("loginAndGenerateToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        //Username and Password are in the DB.Users
        public ActionResult Token(string Username,string Password)
        {
            if (_authService.ValidateLogin(Username, Password))
            {
                var currentDate = DateTime.UtcNow;
                var validity = TimeSpan.FromMinutes(60);
                var expirationDate = currentDate.Add(validity);

                var token = _authService.GenerateToken(currentDate,Username, validity);
                return Ok(new
                {
                    Token = token,
                    ExpireAt = expirationDate
                });
            }
            return Unauthorized();
        }
    }
}
