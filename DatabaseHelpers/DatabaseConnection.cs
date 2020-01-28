using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Npgsql;
using Newtonsoft.Json;
using System.Threading.Tasks;
public class DatabaseConnection 
{
    public async static Task<string> Connection(string query) 
    {
        //var serverName = Environment.GetEnvironmentVariable("SERVER");
        //var port = Environment.GetEnvironmentVariable("PORT");
        //var userId = Environment.GetEnvironmentVariable("USER_ID");
        //var password = Environment.GetEnvironmentVariable("SERVER");
        //var database = Environment.GetEnvironmentVariable("DATABASE");

		

		var connectionString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4}", "82.7.67.210", "5432", "postgres", "recipefordisaster", "recipes");
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
                    ImageUrl = reader["imageurl"].ToString()
                });
				Console.WriteLine(reader["id"]);
				Console.WriteLine(reader["name"]);
            }

        conn.Close();

        var json = JsonConvert.SerializeObject(recipes);

        System.Console.WriteLine(json);     
        return json;
    }
}

public class Step 
{
    public string Id { get; set; }
    public string Description { get; set; }
}

public class Ingredient 
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Recipe 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string ImageUrl { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<Step> Method { get; set; }
}

public class Recipes
{
    public List<Recipe> RecipeList { get; set; }
}