
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
		var myRecipes = JsonConvert.DeserializeObject<Recipes>(recipe);
		var myRecipe = myRecipes.RecipeList[0];

		var myRecipeObject = new RecipeData
		{
			Id = myRecipe.Id,
			Name = myRecipe.Name,
			Description = myRecipe.Description,
			ImageUrl = myRecipe.ImageUrl,
			Ingredients = new List<string>(myRecipe.Ingredients.Split("¬")),
			MethodSteps = new List<string>(myRecipe.MethodSteps.Split("¬"))
		};

		var json = JsonConvert.SerializeObject(myRecipeObject);


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
	public List<string> MethodSteps { get; set; }
	public List<string> Ingredients { get; set; }
}