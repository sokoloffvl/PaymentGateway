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
    [ProducesResponseType(200, Type = typeof(PaymentInfoResponse))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(string paymentId)
    {
        var payment = await paymentService.Get(paymentId);
        if (payment is not null)
            return Ok(payment);
        return NotFound(payment);
    }

    [SwaggerRequestExample(typeof(CreatePaymentRequest), typeof(CardPaymentExampleProvider))]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await paymentService.Create(request);
        return Created($"/{request.PaymentId}", null);
    }
}