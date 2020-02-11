using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        public async Task<string> Post([FromBody] LoginUser loginAttempt)
        {
            var jsonResponse = "";
            var existenceQuery = $"SELECT COUNT(1) FROM users WHERE username = '{loginAttempt.UserName}';";
            var existence = await DatabaseConnection.DoesUserExist(existenceQuery);

            var response = new LoginResponse();
            
            if (!existence)
            {
                response.Status = false;
                jsonResponse = JsonConvert.SerializeObject(response);
                return jsonResponse;
            }

            var query = $"SELECT hashedpassword, salt, id, profilepicture FROM users WHERE username = '{loginAttempt.UserName}';";
            var tableData = await DatabaseConnection.Login(query);
            
            var myHashedPassword = loginAttempt.HashMyPassword(loginAttempt.Password, tableData.Salt);
            var dbHashedPassword = tableData.HashedPassword;
            
            var areEqual = myHashedPassword == dbHashedPassword;

            if(areEqual)
            {
                response.UserId = tableData.Id.ToString();
                response.UserName = loginAttempt.UserName;
                response.ImageUrl = tableData.ProfilePicture;
                if(response.ImageUrl.Length == 0)
                    response.ImageUrl = "http://flatfish.online:38120/images/ProfilePlaceholder.png";
            }

            response.Status = areEqual;

            jsonResponse = JsonConvert.SerializeObject(response);
            return jsonResponse;
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

    public class LoginResponse
    {
        public bool Status { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; } 
    }
}