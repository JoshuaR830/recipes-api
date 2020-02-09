using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Npgsql;
using Newtonsoft.Json;
using System.Threading.Tasks;
public class DatabaseConnection 
{
	public static string connectionString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4}", "82.7.67.210", "5432", "postgres", "recipefordisaster", "recipes");

    public async static Task<string> ShoppingListData(string query) {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
		
        var shoppingData = new ShoppingData(); 

        using (var cmd = new NpgsqlCommand(query, conn))
		using (var reader = await cmd.ExecuteReaderAsync())
        {
			while (await reader.ReadAsync()) {
                shoppingData.UserId = reader["userid"].ToString();
                shoppingData.ShoppingItems = reader["shoppinglist"].ToString();
                shoppingData.TickedItems = reader["ticked"].ToString();
            }
        }

        conn.Close();

        var json = JsonConvert.SerializeObject(shoppingData);
        return json;
    }

    public async static Task<string> Connection(string query) 
    {
        //var serverName = Environment.GetEnvironmentVariable("SERVER");
        //var port = Environment.GetEnvironmentVariable("PORT");
        //var userId = Environment.GetEnvironmentVariable("USER_ID");
        //var password = Environment.GetEnvironmentVariable("SERVER");
        //var database = Environment.GetEnvironmentVariable("DATABASE");

		
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var recipes = new Recipes();
        recipes.RecipeList = new List<Recipe>();


		using (var cmd = new NpgsqlCommand(query, conn))
		using (var reader = await cmd.ExecuteReaderAsync())
			while (await reader.ReadAsync()) {
                recipes.RecipeList.Add(new Recipe {
                    Id = reader["id"].ToString(),
                    Name = reader["name"].ToString(),
                    Description = reader["description"].ToString(),
                    ImageUrl = reader["imageurl"].ToString(),
					Ingredients = reader["ingredients"].ToString(),
					MethodSteps = reader["methodSteps"].ToString()
                });
				Console.WriteLine(reader["id"]);
				Console.WriteLine(reader["name"]);
            }

        conn.Close();

        var json = JsonConvert.SerializeObject(recipes);

        System.Console.WriteLine(json);     
        return json;
    }

    public async static Task<bool> DoesUserExist(string query)
    {
        System.Console.WriteLine(query);
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand(query, conn);
        var count = Convert.ToInt64(cmd.ExecuteScalar());
        conn.Close();
        
        return count >= 1 ? true : false;
    }

    public async static Task<UserData> Login(string query) 
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var userData = new UserData();

		using (var cmd = new NpgsqlCommand(query, conn))
		using (var reader = await cmd.ExecuteReaderAsync())
			while (await reader.ReadAsync()) {
                userData.HashedPassword = reader["hashedpassword"].ToString();
                userData.Salt = reader["salt"].ToString();
            }

        conn.Close();

        return userData;
    }

	public async static Task WriteData(string query)
	{
        System.Console.WriteLine(query);
		var conn = new NpgsqlConnection(connectionString);
		await conn.OpenAsync();

		using (var cmd = new NpgsqlCommand(query, conn))
			cmd.ExecuteNonQuery();

		conn.Close();
	}
}

public class Recipe 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string ImageUrl { get; set; }
    public string Ingredients { get; set; }
    public string MethodSteps { get; set; }
}

public class Recipes
{
    public List<Recipe> RecipeList { get; set; }
}

public class UserData
{
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
}
public class ShoppingData {
    public string UserId { get; set; }
    public string ShoppingItems { get; set; }
    public string TickedItems { get; set; }

}public class UserData
{
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
}