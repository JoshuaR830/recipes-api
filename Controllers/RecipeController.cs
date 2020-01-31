
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

		System.Console.WriteLine(recipes);

		var myRecipes = JsonConvert.DeserializeObject<Recipes>(recipes);

		var recipeIds = new List<RecipeTile>();

		foreach(var recipe in myRecipes.RecipeList)
		{
			recipeIds.Add(new RecipeTile
			{
				Id = recipe.Id,
				Name = recipe.Name,
                Description = recipe.Description,
                ImageUrl = recipe.ImageUrl
			});
		}
		System.Console.WriteLine(recipeIds);
		var json = JsonConvert.SerializeObject(recipeIds);
		System.Console.WriteLine(json);
        return json;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<string>> Get(string id)
    {
        var recipe = await DatabaseConnection.Connection("SELECT * FROM recipes WHERE id='" + id + "';");
		System.Console.WriteLine(">>>>" + recipe);
		var details = JsonConvert.DeserializeObject<RecipeData>(recipe);

		System.Console.WriteLine($"Data {details}");

		var json = JsonConvert.SerializeObject(details);
		System.Console.WriteLine(json);
		return json;
	}
}

class RecipeTile
{
	public string Id { get; set; }
	public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}

class RecipeData
{
	public string Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string ImageUrl { get; set; }
	public string MethodSteps { get; set; }
	public string Ingredients { get; set; }
}