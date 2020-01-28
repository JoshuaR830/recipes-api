
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Http.Cors;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
//[EnableCors(origins: "*", headers: "*", methods: "*")]
public class RecipesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        var recipes = await DatabaseConnection.Connection("SELECT * FROM recipes");

		var myRecipes = JsonConvert.DeserializeObject<Recipes>(recipes);

		var recipeIds = new List<RecipeTile>();

		foreach(var recipe in myRecipes.RecipeList)
		{
			recipeIds.Add(new RecipeTile
			{
				Id = recipe.Id,
				Name = recipe.Name
			});
		}
		System.Console.WriteLine(recipeIds);
		var json = JsonConvert.SerializeObject(recipeIds);
		System.Console.WriteLine(json);
        return json;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> Get(int id)
    {
        var recipe = await DatabaseConnection.Connection("SELECT * FROM recipes WHERE id='" + id + "';");

        return recipe;
    }
}

class RecipeTile
{
	public string Id { get; set; }
	public string Name { get; set; }
}