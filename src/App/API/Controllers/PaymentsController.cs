using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    public PaymentsController()
    {
    }
    
    [HttpGet("{paymentId}")]
    public IActionResult Get(string paymentId)
    {
        return Ok();
    }
}