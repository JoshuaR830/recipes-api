﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
			var query = $"INSERT INTO recipes (id, name, description, imageurl) VALUES ('{Guid.NewGuid()}', '{create.Name}', '{create.Description}', '{create.ImageUrl}')";
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
		public string Name{ get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
	}
}