using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Npgsql;
using Newtonsoft.Json;
using System.Threading.Tasks;
public class DatabaseConnection 
{
	public static string connectionString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4}", "flatfish.online", "5432", "postgres", "recipefordisaster", "recipes");

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

    public async static Task<string> GetScheduledRecipes(string id)
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var daysQuery = $"SELECT * FROM scheduledays WHERE userid = '{id}'";

        var scheduledDays = new ScheduledDayIds();

        scheduledDays.AllDays = new List<string>();
        using (var cmd = new NpgsqlCommand(daysQuery, conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                System.Console.WriteLine(">>>>" + reader["monday"]);
                scheduledDays.AllDays.Add(reader["monday"].ToString());
                scheduledDays.AllDays.Add(reader["tuesday"].ToString());
                scheduledDays.AllDays.Add(reader["wednesday"].ToString());
                scheduledDays.AllDays.Add(reader["thursday"].ToString());
                scheduledDays.AllDays.Add(reader["friday"].ToString());
                scheduledDays.AllDays.Add(reader["saturday"].ToString());
                scheduledDays.AllDays.Add(reader["sunday"].ToString());
            }
        }

        var scheduledTimes = new ScheduledTimeIds();
        scheduledTimes.AllRecipes = new List<DayRecipeIds>();

        foreach(var day in scheduledDays.AllDays)
        {
            var timesQuery = $"SELECT * FROM scheduletimes WHERE dayid = '{day}'";
            using (var cmd = new NpgsqlCommand(timesQuery, conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    scheduledTimes.AllRecipes.Add(new DayRecipeIds {
                        Breakfast = reader["breakfast"].ToString(),
                        Lunch = reader["lunch"].ToString(),
                        Dinner = reader["dinner"].ToString() 
                    });
                }
            }
        }

        var scheduledTimeRecipeList = new ScheduledTimes();
        scheduledTimeRecipeList.AllRecipes = new List<DayRecipes>();
        

        foreach(var time in scheduledTimes.AllRecipes)
        {
            var breakfastId = time.Breakfast;
            var breakfastRecipeQuery = $"SELECT * FROM recipes WHERE id = '{breakfastId}'";
            var breakfastRecipeJson = await Connection(breakfastRecipeQuery);

            var lunchId = time.Lunch;
            var lunchRecipeQuery = $"SELECT * FROM recipes WHERE id = '{lunchId}'";
            var lunchRecipeJson = await Connection(lunchRecipeQuery);
            
            var dinnerId = time.Dinner;
            var dinnerRecipeQuery = $"SELECT * FROM recipes WHERE id = '{dinnerId}'";
            var dinnerRecipeJson = await Connection(dinnerRecipeQuery);

            scheduledTimeRecipeList.AllRecipes.Add(new DayRecipes {
                Breakfast = JsonConvert.DeserializeObject<Recipes>(breakfastRecipeJson).RecipeList[0],
                Lunch = JsonConvert.DeserializeObject<Recipes>(lunchRecipeJson).RecipeList[0],
                Dinner = JsonConvert.DeserializeObject<Recipes>(dinnerRecipeJson).RecipeList[0]
            });
        }

        var scheduleDays = new ScheduledDays();

        scheduleDays.Monday = scheduledTimeRecipeList.AllRecipes[0];
        scheduleDays.Tuesday = scheduledTimeRecipeList.AllRecipes[1];
        scheduleDays.Wednesday = scheduledTimeRecipeList.AllRecipes[2];
        scheduleDays.Thursday = scheduledTimeRecipeList.AllRecipes[3];
        scheduleDays.Friday = scheduledTimeRecipeList.AllRecipes[4];
        scheduleDays.Saturday = scheduledTimeRecipeList.AllRecipes[5];
        scheduleDays.Sunday = scheduledTimeRecipeList.AllRecipes[6];

        return JsonConvert.SerializeObject(scheduleDays);
    }

    internal static async Task<string> GetProfilePicture(string query)
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
		
        var profilePicture = new ProfilePicture();

        using (var cmd = new NpgsqlCommand(query, conn))
		using (var reader = await cmd.ExecuteReaderAsync())
        {
			while (await reader.ReadAsync()) {
                profilePicture.ImageUrl = reader["profilepicture"].ToString();
            }
        }

        conn.Close();

        var json = JsonConvert.SerializeObject(profilePicture);
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
                userData.ProfilePicture = reader["profilepicture"].ToString();
                userData.Id = Guid.Parse(reader["id"].ToString());
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
    public string ProfilePicture { get; set; }
    public Guid Id { get; set; }
}

public class ShoppingData {
    public string UserId { get; set; }
    public string ShoppingItems { get; set; }
    public string TickedItems { get; set; }
}

public class ProfilePicture
{
    public string ImageUrl { get; set; }
}

public class ScheduledDayIds
{
    public List<string> AllDays { get; set; }

}

public class ScheduledTimeIds
{
    public List<DayRecipeIds> AllRecipes { get; set; }
}

public class DayRecipeIds
{
    public string Breakfast { get; set; }
    public string Lunch { get; set; }
    public string Dinner { get; set; }
}

public class ScheduledTimes
{
       public List<DayRecipes> AllRecipes { get; set; }
}

public class DayRecipes
{
    public Recipe Breakfast { get; set; }
    public Recipe Lunch { get; set; }
    public Recipe Dinner { get; set; }
}

public class ScheduledDays
{
    public DayRecipes Monday { get; set; }
    public DayRecipes Tuesday { get; set; }
    public DayRecipes Wednesday { get; set; }
    public DayRecipes Thursday { get; set; }
    public DayRecipes Friday { get; set; }
    public DayRecipes Saturday { get; set; }
    public DayRecipes Sunday { get; set; }
}