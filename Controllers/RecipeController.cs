
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]

public class RecipesController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return "Hello";
    }

    [HttpGet("{id}")]
    public ActionResult<string> Get(int id)
    {
        DatabaseConnection.Connection();
        return id.ToString();
    }
}