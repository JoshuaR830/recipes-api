using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace recipe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduledRecipesController: ControllerBase 
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            var json = await DatabaseConnection.GetScheduledRecipes(id);
            return json;
        }
    }
}