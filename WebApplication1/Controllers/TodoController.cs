using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[AllowAnonymous]
[ApiController]
public class TodoController(IConfiguration configuration) : ControllerBase
{
    private readonly string? _connectionString = configuration.GetConnectionString("mydb");

    [HttpGet("GetTasks")]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = new List<object>(); // You can create a specific model instead of object
        const string query = $"SELECT * FROM todo";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(query, con);
        await con.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(new
            {
                Id = reader["id"],
                Task = reader["task"]
            });
        }
        Console.WriteLine("GetAllTasks");
        return Ok(tasks);
    }

    [HttpPost("AddTask")]
    public async Task<IActionResult> AddTask([FromForm] string task)
    {
        const string query = "INSERT INTO todo (task) VALUES (@task)";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@task", task);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Added Successfully");
        return Ok(new JsonResult("Added Successfully"));
    }

    [HttpPost("DeleteTask")]
    public async Task<IActionResult> DeleteTask([FromForm] int id)
    {
        const string query = "DELETE FROM todo WHERE id = @id";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(query, con);
        cmd.Parameters.AddWithValue("@id", id);
        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Deleted Successfully");
        return Ok(new JsonResult("Deleted Successfully"));
    }
    
    [HttpPost("DeleteAllTasks")]
    public async Task<IActionResult> DeleteAllTasks()
    {
        const string query = "DELETE FROM todo";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(query, con);
        await con.OpenAsync();
        int rowsAffected = await cmd.ExecuteNonQueryAsync();

        return Ok(new JsonResult($"{rowsAffected} task(s) deleted successfully."));
    }
}