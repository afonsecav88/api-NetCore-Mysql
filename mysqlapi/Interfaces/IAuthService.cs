
using System;

namespace mysqlapi.Interfaces
{
    public interface IAuthService
    {
        bool ValidateLogin(string username, string password);
        string GenerateToken(DateTime currentDate, string username, TimeSpan timeValidity);
    }
}
