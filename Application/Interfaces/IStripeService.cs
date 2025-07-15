using Application.DTOs.CheckoutDTOs;
using Domain.Entities;
using Stripe;
using Stripe.Checkout;

namespace Application.Interfaces;

public interface IStripeService
{
    Task<Customer> CreateCustomerAsync(string email, string name, string phone);
    Task<PaymentMethod> CreatePaymentMethodAsync(CardInfo card);
    // Task<PaymentIntent> ConfirmPaymentIntentAsync(string customerId, long amount, string currency, string paymentMethodId);
    
    Task<Session> CreateCheckoutSessionAsync(
        CheckoutSessionRequestDto request,
        decimal shippingCost,
        string shippingLabel,
        string successUrl,
        string cancelUrl);
    Event ConstructEvent(string json, string signature, string secret);
    Task<Session> GetSessionAsync(string sessionId);
}