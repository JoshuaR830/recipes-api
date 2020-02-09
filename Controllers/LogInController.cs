using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace recipe_api.Controllers 
{    
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase 
    {
        [HttpGet("{userName}")]
        public async Task<ActionResult<string>> Get(string userName)
        {
            var query = $"SELECT hashedpassword, salt FROM users WHERE username = '{userName}';";
            var userData = await DatabaseConnection.Login(query);
            return "Not implemented";
        }

        [HttpPost]
        public async Task<bool> Post([FromBody] LoginUser loginAttempt)
        {
            var existenceQuery = $"SELECT COUNT(1) FROM users WHERE username = '{loginAttempt.UserName}';";
            var existence = await DatabaseConnection.DoesUserExist(existenceQuery);

            if (!existence)
                return false;

            var query = $"SELECT hashedpassword, salt FROM users WHERE username = '{loginAttempt.UserName}';";
            var tableData = await DatabaseConnection.Login(query);
            
            var myHashedPassword = loginAttempt.HashMyPassword(loginAttempt.Password, tableData.Salt);
            var dbHashedPassword = tableData.HashedPassword;
            
            var areEqual = myHashedPassword == dbHashedPassword;
            return areEqual;
        }
    } 

    public class LoginUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string HashMyPassword(string password, string salt)
        {
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            
            var saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);
            var sha256 = SHA256.Create();

            var hashedPassword = sha256.ComputeHash(saltedPassword);

            var builder = new StringBuilder();
            Array.ForEach(hashedPassword, e => builder.Append(e.ToString("x2")));
            return builder.ToString();
        }
    }
}