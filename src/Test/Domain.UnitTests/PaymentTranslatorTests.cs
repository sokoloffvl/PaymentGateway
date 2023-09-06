using AutoFixture;
using Domain.Models;
using Domain.Translators;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.UnitTests;

public class PaymentTranslatorTests
{
    private Fixture autoFixture;
    [SetUp]
    public void Setup()
    {
        autoFixture = new Fixture();
    }
    
    [Test]
    public void CanTranslateBankPaymentRequest()
    {
        //Arrange
        var payment = autoFixture
            .Build<Payment>()
            .Create();
        //Act
        var translated = payment.ToRequest();
        //Assert
        translated.PaymentId.Should().Be(payment.PaymentId);
        translated.CardNumber.Should().Be(payment.CardDetails.Number);
        translated.CardOwner.Should().Be(payment.CardDetails.Owner);
        translated.CVV.Should().Be(payment.CardDetails.CVV);
        translated.ValidToYear.Should().Be(payment.CardDetails.ValidToYear);
        translated.ValidToMonth.Should().Be(payment.CardDetails.ValidToMonth);
        translated.Amount.Should().Be(payment.Amount);
        translated.Currency.Should().Be(payment.Currency);
    }

}