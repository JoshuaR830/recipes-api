using System;
using System.Data.SqlClient;
using System.Collections.Generic;

public class DatabaseConnection 
{
    public static void Connection() 
    {
        var serverName = Environment.GetEnvironmentVariable("SERVER");
        var port = Environment.GetEnvironmentVariable("PORT");
        var userId = Environment.GetEnvironmentVariable("USER_ID");
        var password = Environment.GetEnvironmentVariable("SERVER");
        var database = Environment.GetEnvironmentVariable("DATABASE");

        var connectionString = $"server={serverName},{port};user id={userId}; password={password}; database={database};";
        var conn = new SqlConnection(connectionString);
        conn.Open();
    }
}