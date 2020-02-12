using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace recipe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase 
    {
        [HttpPost]
        public async Task<string> Post([FromBody] string id) 
        {
            var jsonResponse = "";
            var existenceQuery = $"SELECT COUNT(1) FROM users WHERE id = '{id}';";
            var existence = await DatabaseConnection.DoesUserExist(existenceQuery);

            var response = new LoginResponse();

            if (!existence)
            {
                response.Status = false;
                jsonResponse = JsonConvert.SerializeObject(response);
                return jsonResponse;
            }

            var query = $"SELECT, username, profilepicture FROM users WHERE id = '{id}';";
            var tableData = await DatabaseConnection.Login(query);
        
            response.Status = true;
            response.UserId = id;
            response.UserName = tableData.UserName;
            response.ImageUrl = tableData.ProfilePicture;
            if(response.ImageUrl.Length == 0)
                response.ImageUrl = "http://flatfish.online:38120/images/ProfilePlaceholder.png";

            var responseData = JsonConvert.SerializeObject(response);
            return responseData;
        }
    }
}