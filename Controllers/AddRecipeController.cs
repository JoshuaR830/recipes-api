using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using recipe_api.Helpers;

namespace recipe_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddRecipeController : ControllerBase
	{
		[HttpPost]
		public async Task Post([FromBody] Create create)
		{
			var methodSteps = ListConversionManager.ConvertToString(create.MethodSteps);
			var ingredients = ListConversionManager.ConvertToString(create.Ingredients);

			var query = $"INSERT INTO recipes (id, name, description, imageurl, ingredients, methodSteps) VALUES ('{Guid.NewGuid()}', '{create.Name}', '{create.Description}', '{create.ImageUrl}', '{ingredients}', '{methodSteps}')";
			await DatabaseConnection.WriteData(query);
		}

		[HttpPut("{id}")]
		public async Task Put(Guid id, [FromBody] Update update)
		{
			var methodSteps = ListConversionManager.ConvertToString(update.MethodSteps);
			var ingredients = ListConversionManager.ConvertToString(update.Ingredients);

			var query = $"UPDATE recipes SET name = '{update.Name}', description = '{update.Description}', imageUrl = '{update.ImageUrl}', ingredients = '{ingredients}', methodSteps = '{methodSteps}' WHERE id = '{id}';";
			await DatabaseConnection.WriteData(query);
		}
	}

	public class Create
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public List<string> MethodSteps { get; set; }
		public List<string> Ingredients { get; set; }
	}

	public class Update
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public List<string> MethodSteps { get; set; }
		public List<string> Ingredients { get; set; }
	}
}
