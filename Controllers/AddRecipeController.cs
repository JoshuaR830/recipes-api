﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace recipe_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    public class AddRecipeController : ControllerBase
	{
		[HttpPost]
		public async Task Post([FromBody] Create create)
		{
			Console.WriteLine("Value >>> " + create);

			var ingredients = JsonConvert.SerializeObject(create.Ingredients);
			var methodSteps = JsonConvert.SerializeObject(create.MethodSteps);

			Console.WriteLine(ingredients);
			Console.WriteLine(methodSteps);

			var query = $"INSERT INTO recipes (id, name, description, imageurl, ingredients, methodSteps) VALUES ('{Guid.NewGuid()}', '{create.Name}', '{create.Description}', '{create.ImageUrl}', '{ingredients}', '{methodSteps}')";
			await DatabaseConnection.WriteData(query);
		}

		[HttpPut]
		public async Task Put([FromBody] string value)
		{
			Console.WriteLine("Value >>> " + value);
			var query = $"INSERT INTO recipes (id, name, description, imageurl) VALUES ('12345', 'newerer recipe', 'An even newer recipe than the newer one', 'http://flatfish.online:38120/images/AtUllswater.png')";
			await DatabaseConnection.WriteData(query);
		}
	}

	public class Create
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public JObject MethodSteps { get; set; }
		public JObject Ingredients { get; set; }
	}
}
