using System;
using System.Threading.Tasks;
using AutoFixture;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Domain.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Domain.UnitTests;

public class PaymentProcessingServiceTest
{
    private IPaymentProcessingService sut;
    private Mock<IBankClient> mockBankClient;
    private Mock<IPaymentRepository> mockPaymentRepository;
    private Fixture autoFixture;
    
    [SetUp]
    public void SetUp()
    {
        autoFixture = new Fixture();
        mockBankClient = new Mock<IBankClient>();
        mockPaymentRepository = new Mock<IPaymentRepository>();
        sut = new PaymentProcessingService(mockBankClient.Object, mockPaymentRepository.Object);
    }

    [Test]
    public async Task GivenPaymentId_WhenPaymentDoesntExist_ThenDoNothing()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        mockPaymentRepository.Setup(r => r.GetPaymentAsync(paymentId)).ReturnsAsync((Payment)null);
        //Act
        await sut.Process(paymentId);
        //Assert
        mockPaymentRepository.Verify(r => r.GetPaymentAsync(paymentId), Times.Once);
        mockPaymentRepository.Verify(r => r.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Never);
        mockBankClient.Verify(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>()), Times.Never);
    }
    
    [TestCase(PaymentStatus.Processing)]
    [TestCase(PaymentStatus.Declined)]
    [TestCase(PaymentStatus.Succeeded)]
    [TestCase(PaymentStatus.ProcessingFailed)]
    public async Task GivenPaymentId_WhenPaymentAlreadyProcessed_ThenDoNothing(PaymentStatus paymentStatus)
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = autoFixture.Build<Payment>()
            .Create();
        payment.Processed(paymentStatus);
        mockPaymentRepository.Setup(r => r.GetPaymentAsync(paymentId)).ReturnsAsync(payment);
        //Act
        await sut.Process(paymentId);
        //Assert
        mockPaymentRepository.Verify(r => r.GetPaymentAsync(paymentId), Times.Once);
        mockPaymentRepository.Verify(r => r.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Never);
        mockBankClient.Verify(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>()), Times.Never);
    }
    
    [Test]
    public async Task GivenPaymentId_WhenBankReturnsProcessedStatus_ThenPaymentIsProcessed()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = autoFixture.Build<Payment>()
            .Create();
        payment.Created();
        var bankResponse = new BankPaymentResponse
        {
            PaymentId = paymentId,
            Status = PaymentStatus.Succeeded,
            DeclineReason = null
        };
        mockPaymentRepository.Setup(r => r.GetPaymentAsync(paymentId)).ReturnsAsync(payment);
        mockBankClient.Setup(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>())).ReturnsAsync(bankResponse);
        //Act
        await sut.Process(paymentId);
        //Assert
        payment.Status.Should().Be(PaymentStatus.Succeeded);
        mockPaymentRepository.Verify(r => r.GetPaymentAsync(paymentId), Times.Once);
        mockPaymentRepository.Verify(r => r.UpdatePaymentAsync(payment), Times.Once);
        mockBankClient.Verify(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>()), Times.Once);
    }
    
    [TestCase(DeclineReason.FraudDetected)]
    [TestCase(DeclineReason.InsufficientFunds)]
    [TestCase(DeclineReason.InvalidCardDetails)]
    public async Task GivenPaymentId_WhenBankReturnsDeclinedStatus_ThenPaymentIsDeclinedWithDeclineReason(DeclineReason declineReason)
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = autoFixture.Build<Payment>()
            .Create();
        payment.Created();
        var bankResponse = new BankPaymentResponse
        {
            PaymentId = paymentId,
            Status = PaymentStatus.Declined,
            DeclineReason = declineReason
        };
        mockPaymentRepository.Setup(r => r.GetPaymentAsync(paymentId)).ReturnsAsync(payment);
        mockBankClient.Setup(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>())).ReturnsAsync(bankResponse);
        //Act
        await sut.Process(paymentId);
        //Assert
        payment.Status.Should().Be(PaymentStatus.Declined);
        payment.DeclineReason.Should().Be(declineReason);
        mockPaymentRepository.Verify(r => r.GetPaymentAsync(paymentId), Times.Once);
        mockPaymentRepository.Verify(r => r.UpdatePaymentAsync(payment), Times.Once);
        mockBankClient.Verify(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>()), Times.Once);
    }
    
    [Test]
    public async Task GivenPaymentId_WhenBankCantProcessRequestTwice_ThenRequestIsProcessAfterThirdRetry()
    {
        //Arrange
        var paymentId = Guid.NewGuid().ToString();
        var payment = autoFixture.Build<Payment>()
            .Create();
        payment.Created();
        var failedBankResponse = new BankPaymentResponse
        {
            PaymentId = paymentId,
            Status = PaymentStatus.ProcessingFailed,
        };
        var processedBankResponse = new BankPaymentResponse
        {
            PaymentId = paymentId,
            Status = PaymentStatus.Succeeded,
        };
        mockPaymentRepository.Setup(r => r.GetPaymentAsync(paymentId)).ReturnsAsync(payment);
        mockBankClient.SetupSequence(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>()))
            .ReturnsAsync(failedBankResponse)
            .ReturnsAsync(failedBankResponse)
            .ReturnsAsync(processedBankResponse);
        //Act
        await sut.Process(paymentId);
        //Assert
        payment.Status.Should().Be(PaymentStatus.Succeeded);
        payment.DeclineReason.Should().BeNull();
        mockPaymentRepository.Verify(r => r.GetPaymentAsync(paymentId), Times.Once);
        mockPaymentRepository.Verify(r => r.UpdatePaymentAsync(payment), Times.Once);
        mockBankClient.Verify(c => c.ProcessPayment(It.IsAny<BankPaymentRequest>()), Times.Exactly(3));
    }

}