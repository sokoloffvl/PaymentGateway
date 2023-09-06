using System;
using System.Threading.Tasks;
using AutoFixture;
using Domain.Interfaces;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Repository;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;

namespace Application.UnitTests;

public class InMemoryPaymentRepositoryTests
{
    private IPaymentRepository sut;
    private IMemoryCache memCache;
    private Fixture autoFixture;
    
    [SetUp]
    public void Setup()
    {
        autoFixture = new Fixture();
        memCache = new MemoryCache(new MemoryCacheOptions());
        sut = new InMemoryPaymentRepository(memCache);
    }

    [Test]
    public async Task GivenPaymentId_WhenIdNotInCache_ThenGetPaymentReturnsNull()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        //Act
        var payment = await sut.GetPaymentAsync(paymentId);
        //Assert
        payment.Should().BeNull();
    }
    
    [Test]
    public async Task GivenPaymentId_WhenIdExistsInCache_ThenGetPaymentReturnsPayment()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var existingPayment = autoFixture
            .Build<Payment>()
            .With(p => p.PaymentId, paymentId)
            .Create();
        memCache.Set(paymentId, existingPayment);
        //Act
        var payment = await sut.GetPaymentAsync(paymentId);
        //Assert
        payment.Should().Be(existingPayment);
    }
    
    [Test]
    public async Task GivenPaymentId_WhenIdExistsInCache_ThenCreatePaymentThrows()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var existingPayment = autoFixture
            .Build<Payment>()
            .With(p => p.PaymentId, paymentId)
            .Create();
        memCache.Set(paymentId, existingPayment);
        //Act
        var act = async () => await sut.CreatePaymentAsync(existingPayment);
        //Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"Duplicate key {paymentId} found");
    }
    
    [Test]
    public async Task GivenPaymentId_WhenIdDoesntExistInCache_ThenCreatePaymentAddsPayment()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = autoFixture
            .Build<Payment>()
            .With(p => p.PaymentId, paymentId)
            .Create();
        //Act
        var act = async () => await sut.CreatePaymentAsync(payment);
        //Assert
        await act.Should().NotThrowAsync<InvalidOperationException>();
        memCache.TryGetValue(paymentId, out _).Should().BeTrue();
    }
    
    [Test]
    public async Task GivenPaymentId_WhenIdExistsInCache_ThenUpdatePaymentSucceeds()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var existingPayment = autoFixture
            .Build<Payment>()
            .With(p => p.PaymentId, paymentId)
            .Create();
        var updatedPayment = autoFixture
            .Build<Payment>()
            .With(p => p.PaymentId, paymentId)
            .Create();
        memCache.Set(paymentId, existingPayment);
        //Act
        var act = async () => await sut.UpdatePaymentAsync(updatedPayment);
        //Assert
        await act.Should().NotThrowAsync<ArgumentException>();
        memCache.TryGetValue(paymentId, out var newPayment).Should().BeTrue();
        newPayment.Should().Be(updatedPayment);
    }
    
    [Test]
    public async Task GivenPaymentId_WhenIdDoesntExistInCache_ThenUpdatePaymentThrows()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = autoFixture
            .Build<Payment>()
            .With(p => p.PaymentId, paymentId)
            .Create();
        //Act
        var act = async () => await sut.UpdatePaymentAsync(payment);
        //Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage($"Payment {payment.PaymentId} not found");
    }
    
}