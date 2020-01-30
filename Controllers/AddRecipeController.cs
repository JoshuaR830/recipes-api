﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

			var methodSteps = ListToObject(create.MethodSteps);
			var ingredients = ListToObject(create.Ingredients);

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

		private string ListToObject(List<string> data)
		{
			var myObject = "";
			for (var i = 0; i < data.Count; i++)
			{
				if (i == 0)
					myObject += "{\"" + data[i] + "\",";
				else if (i < data.Count - 1)
					myObject += "\"" + data[i] + "\",";
				else
					myObject += "\"" + data[i] + "\"}";
			}

			Console.WriteLine(myObject);

			return myObject;
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
}
