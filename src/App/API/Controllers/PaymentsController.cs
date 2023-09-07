using API.ExampleProviders;
using API.Models;
using API.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace API.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        this.paymentService = paymentService;
    }
    
    [HttpGet("{paymentId}")]
    public async Task<IActionResult> Get(string paymentId)
    {
        var payment = await paymentService.Get(paymentId);
        if (payment is not null)
            return Ok(payment);
        return NotFound(payment);
    }

    [SwaggerRequestExample(typeof(CreatePaymentRequest), typeof(CardPaymentExampleProvider))]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await paymentService.Create(request);
        return Ok();
    }
}