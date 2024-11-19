using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult GetTestMessage()
    {
        return Ok(new { message = "idk wtf is that" });
    }

    
}
