using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace recipe_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    public class ShoppingListController : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<string>> Get()
		{
			string query = "SELECT * FROM shoppingList where userid='b645d320-ae7f-42b6-acf6-5af52693ffa6'";
			var jsonData = await DatabaseConnection.ShoppingListData(query);

			var shoppingList = JsonConvert.DeserializeObject<ShoppingData>(jsonData);

			System.Console.WriteLine(shoppingList.TickedItems);
			var tickList = new List<string>(shoppingList.TickedItems.Split("::"));
			System.Console.WriteLine(tickList);
			var ticked = new List<int>();
			foreach(var tick in tickList) 
			{
				System.Console.WriteLine("Tick: " + tick);
				if(tick.Length > 0)
					ticked.Add(Convert.ToInt32(tick));
			}

			var shoppingListData = new ShoppingListData
			{
				UserId = Guid.Parse(shoppingList.UserId),
				ShoppingList = new List<string>(shoppingList.ShoppingItems.Split("::")),
				Ticked = ticked
			};

			return JsonConvert.SerializeObject(shoppingListData);
		}

		[HttpPut("{id}")]
		public async Task Put(string id, [FromBody] ShoppingListData shoppingList)
		{
			var shoppingListItems = string.Join("::", shoppingList.ShoppingList);
			var ticked = string.Join("::", shoppingList.Ticked);
			var query = $"UPDATE shoppinglist SET shoppinglist = '{shoppingListItems}', ticked = '{ticked}' WHERE userId = '{shoppingList.UserId}'";
			await DatabaseConnection.WriteData(query);
		}

		[HttpPost]
		public async Task Post([FromBody] ShoppingListData shoppingList)
		{
			var query = $"INSERT INTO shoppinglist (userid) VALUES ('')";
			await DatabaseConnection.WriteData(query);
		}
	}

	public class ShoppingListData
	{
		public Guid UserId { get; set; }
		public List<string> ShoppingList { get; set; }
		public List<int> Ticked { get; set; }
	}
}
