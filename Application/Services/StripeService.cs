using Application.DTOs.CheckoutDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;
using Stripe;

public class StripeService : IStripeService
{
    private readonly ILogger _logger;
    private readonly StripeClient _stripeClient;

    public StripeService(ILogger<StripeService> logger)
    {
        _logger = logger;

        var secretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Stripe secret key not configured in environment variables.");
        }

        _stripeClient = new StripeClient(secretKey);
    }

    public async Task<Customer> CreateCustomerAsync(string email, string name, string phone)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name,
            Phone = phone
        };

        var service = new CustomerService(_stripeClient);
        return await service.CreateAsync(options);
    }

    public async Task<PaymentMethod> CreatePaymentMethodAsync(CardInfo card)
    {
        var options = new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions
            {
                Number = card.CardNumber,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
                Cvc = card.CVV
            }
        };

        var service = new PaymentMethodService(_stripeClient);
        return await service.CreateAsync(options);
    }

    public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string customerId, long amount, string currency, string paymentMethodId)
    {
        var options = new PaymentIntentCreateOptions
        {
            Customer = customerId,
            Amount = amount,
            Currency = currency,
            PaymentMethod = paymentMethodId,
            Confirm = true
        };

        var service = new PaymentIntentService(_stripeClient);
        return await service.CreateAsync(options);
    }

    public async Task<Session> CreateCheckoutSessionAsync(CheckoutSessionRequestDto request, decimal shippingCost, string shippingLabel, string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            CustomerEmail = request.CustomerEmail,
            LineItems = request.LineItems
                .Select(li => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = li.PriceData.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = li.PriceData.ProductData.Name,
                            Metadata = li.PriceData.ProductData.Metadata
                        },
                        UnitAmount = li.PriceData.UnitAmount
                    },
                    Quantity = li.Quantity
                })
                .Append(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = shippingLabel
                        },
                        UnitAmount = (long)(shippingCost * 100)
                    },
                    Quantity = 1
                })
                .ToList(),
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "phone", request.CustomerPhone }
                }
            }
        };

        var service = new SessionService(_stripeClient);
        return await service.CreateAsync(options);
    }

    public Event ConstructEvent(string json, string signature, string secret)
    {
        return EventUtility.ConstructEvent(json, signature, secret);
    }

    public async Task<Session> GetSessionAsync(string sessionId)
    {
        var service = new SessionService(_stripeClient);
        return await service.GetAsync(sessionId);
    }
}
