using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace recipe_api.Controllers 
{    
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase 
    {
        [HttpGet("{userName}")]
        public async Task<ActionResult<string>> Get(string userName)
        {
            System.Console.WriteLine(userName);

            var query = $"SELECT hashedpassword, salt FROM users WHERE username = '{userName}';";
            var userData = await DatabaseConnection.Login(query);
            return "Not implemented";
        }

        [HttpPost]
        public async Task<string> Post([FromBody] RegisterUser register)
        {
            var hashUser = new HashUser(register.Password);

            var existenceQuery = $"SELECT COUNT(1) FROM users WHERE username = '{register.UserName}';";
            var existence = await DatabaseConnection.DoesUserExist(existenceQuery);

            System.Console.WriteLine(existence);

            var registeredResponse = new RegisterResponse();


            if (existence) {
                registeredResponse.Status = false;
                return JsonConvert.SerializeObject(registeredResponse);
            }

            var hashedPassword = hashUser.HashedPassword;
            var salt = Encoding.ASCII.GetString(hashUser.Salt);

            var query = $"INSERT INTO users (username, hashedpassword, salt) VALUES ('{register.UserName}', '{hashedPassword}', '{salt}');";
			await DatabaseConnection.WriteData(query);

            registeredResponse.Status = true;

            // make a request to db for user data
            registeredResponse.UserId = Guid.NewGuid();
            registeredResponse.UserName = "Joshua";
            registeredResponse.ImageUrl = "http://flatfish.online:38120/images/Facebook%20Profile.png";
            
            return JsonConvert.SerializeObject(registeredResponse);
        }
    } 

    public class RegisterUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class HashUser
    {
        public string UserName { get; set; }
        public byte[] Salt { get; set; }
        public string HashedPassword { get; set; }

        public HashUser(string password) {
            this.Salt = GenerateSalt();
            this.HashedPassword = GenerateHashedPassword(password);
        }

        public byte[] GenerateSalt() 
        {
            var randomGuid = Guid.NewGuid();
            var base64Guid = System.Convert.ToBase64String(randomGuid.ToByteArray());
            var saltBytes = Encoding.ASCII.GetBytes(base64Guid.ToString());
            System.Console.WriteLine(saltBytes);
            return saltBytes;
        }

        public byte[] GenerateSaltedPassword(string password)
        {
            var saltBytes = this.Salt;
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            
            var saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);

            return saltedPassword;
        }

        public string GenerateHashedPassword(string plainPassword)
        {
            var saltedPasswordBytes = GenerateSaltedPassword(plainPassword);
            var sha256 = SHA256.Create();
            var hashedPassword = sha256.ComputeHash(saltedPasswordBytes);
            var builder = new StringBuilder();
            Array.ForEach(hashedPassword, e => builder.Append(e.ToString("x2")));
            return builder.ToString();
        }

        public bool ComparePasswords(string enteredPassword)
        {
            var myPassword = GenerateHashedPassword(enteredPassword);

            System.Console.WriteLine($"Original:{this.HashedPassword}");
            System.Console.WriteLine($"Entered: {myPassword}");
            System.Console.WriteLine($"Salt: {Encoding.ASCII.GetString(this.Salt)}");
            System.Console.WriteLine(this.HashedPassword == myPassword);
            return myPassword.SequenceEqual(this.HashedPassword);
        }
    }

    public class RegisterResponse
    {
        public bool Status { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public Guid UserId { get; set; } 
    }
}