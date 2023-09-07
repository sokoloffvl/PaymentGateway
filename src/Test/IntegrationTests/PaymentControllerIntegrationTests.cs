using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace IntegrationTests;

public class PaymentControllerIntegrationTests
{
    private WebApplicationFactory<Program> factory;
    private HttpClient client;
    
    [SetUp]
    public void Setup()
    {
        factory = new WebApplicationFactory<Program>();
        client = factory.CreateClient();
    }

    [Test]
    public async Task GivenPaymentId_WhenUnauthorized_ThenGetPaymentReturns401()
    {
        //Arrange
        var paymentId = "1234";
        //Act
        var resp = await client.GetAsync($"Payments/{paymentId}");
        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Test]
    public async Task GivenPaymentId_WhenPaymentDoestExist_ThenGetPaymentReturns404()
    {
        //Arrange
        var paymentId = "1234";
        var request = new HttpRequestMessage() {
            
            RequestUri = new Uri(client.BaseAddress,$"Payments/{paymentId}"),
            Method = HttpMethod.Get,
        };
        request.Headers.Add("x-api-key", "one");
        //Act
        var resp = await client.SendAsync(request);
        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Test]
    public async Task GivenPaymentRequest_WhenUnauthorized_ThenCreatePaymentReturns401()
    {
        //Arrange
        var paymentRequest = new CreatePaymentRequest
        {
            PaymentId = Guid.NewGuid().ToString(),
            CardNumber = "378282246310005",
            CVV = "123",
            CardOwner = "Card Owner",
            Amount = 100,
            Currency = "USD",
            ValidToYear = 29,
            ValidToMonth = 11
        };
        var request = new HttpRequestMessage() {
            Content = JsonContent.Create(paymentRequest),
            RequestUri = new Uri(client.BaseAddress,$"Payments"),
            Method = HttpMethod.Post,
        };
        //Act
        var resp = await client.SendAsync(request);
        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Test]
    public async Task GivenPaymentRequest_WhenAuthorized_ThenCreatePaymentReturns201()
    {
        //Arrange
        var paymentRequest = new CreatePaymentRequest
        {
            PaymentId = Guid.NewGuid().ToString(),
            CardNumber = "378282246310005",
            CVV = "123",
            CardOwner = "Card Owner",
            Amount = 100,
            Currency = "USD",
            ValidToYear = 29,
            ValidToMonth = 11
        };
        var request = new HttpRequestMessage() {
            Content = JsonContent.Create(paymentRequest),
            RequestUri = new Uri(client.BaseAddress,$"Payments"),
            Method = HttpMethod.Post,
        };
        request.Headers.Add("x-api-key", "one");
        //Act
        var resp = await client.SendAsync(request);
        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Test]
    public async Task GivenPaymentRequest_WhenRequestIsInvalid_ThenCreatePaymentReturns400()
    {
        //Arrange
        var paymentRequest = new CreatePaymentRequest
        {
            PaymentId = Guid.NewGuid().ToString(),
            CardNumber = "0",
        };
        var request = new HttpRequestMessage() {
            Content = JsonContent.Create(paymentRequest),
            RequestUri = new Uri(client.BaseAddress,$"Payments"),
            Method = HttpMethod.Post,
        };
        request.Headers.Add("x-api-key", "one");
        //Act
        var resp = await client.SendAsync(request);
        //Assert
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Test]
    public async Task GivenPaymentRequest_WhenRequestIsValid_ThenGetReturnsValidResponseAfterCreation()
    {
        //Arrange
        var paymentRequest = new CreatePaymentRequest
        {
            PaymentId = Guid.NewGuid().ToString(),
            CardNumber = "378282246310005",
            CVV = "123",
            CardOwner = "Card Owner",
            Amount = 100,
            Currency = "USD",
            ValidToYear = 29,
            ValidToMonth = 11
        };
        var postRequest = new HttpRequestMessage() {
            Content = JsonContent.Create(paymentRequest),
            RequestUri = new Uri(client.BaseAddress,$"Payments"),
            Method = HttpMethod.Post,
        };
        var getRequest = new HttpRequestMessage() {
            RequestUri = new Uri(client.BaseAddress,$"Payments/{paymentRequest.PaymentId}"),
            Method = HttpMethod.Get,
        };
        postRequest.Headers.Add("x-api-key", "one");
        getRequest.Headers.Add("x-api-key", "one");
        //Act
        var postResp = await client.SendAsync(postRequest);
        var getResp = await client.SendAsync(getRequest);
        //Assert
        var content = await getResp.Content.ReadAsStringAsync();
        var paymentResponse = JsonConvert.DeserializeObject<PaymentInfoResponse>(content);
        postResp.StatusCode.Should().Be(HttpStatusCode.Created);
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.PaymentId.Should().Be(paymentRequest.PaymentId);
        paymentResponse.Amount.Should().Be(paymentRequest.Amount);
        paymentResponse.Currency.Should().Be(paymentRequest.Currency);
        paymentResponse.CardNumber.Should().Be("3782 *** 0005");
    }
    
    [Test]
    public async Task GivenPaymentRequest_WhenRequestIsValid_ThenPaymentStatusShouldBeUpdatedAfter15Seconds()
    {
        //Arrange
        var paymentRequest = new CreatePaymentRequest
        {
            PaymentId = Guid.NewGuid().ToString(),
            CardNumber = "378282246310005",
            CVV = "123",
            CardOwner = "Card Owner",
            Amount = 100,
            Currency = "USD",
            ValidToYear = 29,
            ValidToMonth = 11
        };
        var postRequest = new HttpRequestMessage() {
            Content = JsonContent.Create(paymentRequest),
            RequestUri = new Uri(client.BaseAddress,$"Payments"),
            Method = HttpMethod.Post,
        };
        var getRequest = new HttpRequestMessage() {
            RequestUri = new Uri(client.BaseAddress,$"Payments/{paymentRequest.PaymentId}"),
            Method = HttpMethod.Get,
        };
        postRequest.Headers.Add("x-api-key", "one");
        getRequest.Headers.Add("x-api-key", "one");
        //Act
        var postResp = await client.SendAsync(postRequest);
        await Task.Delay(TimeSpan.FromSeconds(15));
        var getResp = await client.SendAsync(getRequest);
        //Assert
        var content = await getResp.Content.ReadAsStringAsync();
        var paymentResponse = JsonConvert.DeserializeObject<PaymentInfoResponse>(content);
        postResp.StatusCode.Should().Be(HttpStatusCode.Created);
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.PaymentId.Should().Be(paymentRequest.PaymentId);
        paymentResponse.Status.Should().BeOneOf("ProcessingFailed", "Succeeded", "Declined");
        paymentResponse.DeclineReason.Should().BeOneOf("None", "InsufficientFunds", "InvalidCardDetails", "FraudDetected");
    }
}