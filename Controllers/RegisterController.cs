using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace recipe_api.Controllers 
{    
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase 
    {
        [HttpGet("{userName}")]
        public async Task<ActionResult<string>> Get(string userName)
        {
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

            var userId = Guid.NewGuid().ToString();
            var query = $"INSERT INTO users (id, username, hashedpassword, salt, profilepicture) VALUES ('{userId}', '{register.UserName}', '{hashedPassword}', '{salt}', '{register.ImageUrl}' );";
			await DatabaseConnection.WriteData(query);

            registeredResponse.Status = true;
            var createShoppingListQuery = $"INSERT INTO shoppinglist (userid) VALUES ('{userId.ToString()}')";
            await DatabaseConnection.WriteData(createShoppingListQuery);

            var createScheduleQuery = $"INSERT INTO scheduledays (userid) VALUES ('{userId.ToString()}')";

            var days = new List<string> {"monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"};

            foreach(var day in days)
            {
                var dayId = Guid.NewGuid();
                var createDayQuery = $"INSERT INTO scheduletimes (dayid) VALUES ('{dayId}')";
                var updateDayQuery = $"UPDATE scheduledays SET {day} = '{dayId}' WHERE userid = '{userId.ToString()}'";
                await DatabaseConnection.WriteData(createDayQuery);
                await DatabaseConnection.WriteData(updateDayQuery);
            }

            registeredResponse.UserId = Guid.Parse(userId);
            registeredResponse.UserName = register.UserName;
            registeredResponse.ImageUrl = register.ImageUrl;
            
            return JsonConvert.SerializeObject(registeredResponse);
        }
    } 

    public class RegisterUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
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