﻿using API.Models;
using API.Translators;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.EventBus;

namespace API.Services;

public interface IPaymentService
{
    Task Create(CreatePaymentRequest request);
    Task<PaymentInfoResponse?> Get(string paymentId);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository paymentRepository;
    private readonly IMaskingService maskingService;
    private readonly IBackgroundQueue<Payment> backgroundQueue;

    public PaymentService(IPaymentRepository paymentRepository,
        IMaskingService maskingService,
        IBackgroundQueue<Payment> backgroundQueue)
    {
        this.paymentRepository = paymentRepository;
        this.maskingService = maskingService;
        this.backgroundQueue = backgroundQueue;
    }

    public async Task Create(CreatePaymentRequest request)
    {
        try
        {
            var payment = request.ToDomain();
            await paymentRepository.CreatePaymentAsync(payment);
            backgroundQueue.Enqueue(payment);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<PaymentInfoResponse?> Get(string paymentId)
    {
        try
        {
            var payment = await paymentRepository.GetPaymentAsync(paymentId);
            if (payment is null)
                return null;
            var cardNumber = maskingService.MaskCardNumber(payment.CardDetails.Number);
            return new PaymentInfoResponse
            {
                PaymentId = payment.PaymentId,
                CardNumber = cardNumber,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status.ToString(),
                DeclineReason = payment.DeclineReason is null ? "None" : payment.DeclineReason.ToString()
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}