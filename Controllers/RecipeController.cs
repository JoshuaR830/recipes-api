
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Web.Http.Cors;

[Route("api/[controller]")]
[ApiController]
//[EnableCors(origins: "*", headers: "*", methods: "*")]
public class RecipesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        var recipes = await DatabaseConnection.Connection("SELECT * FROM recipes");

        return recipes;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> Get(int id)
    {
        var recipe = await DatabaseConnection.Connection("SELECT * FROM recipes WHERE id='" + id + "';");

        return recipe;
    }
}