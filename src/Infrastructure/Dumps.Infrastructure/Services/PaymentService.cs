using Dumps.Application.ServicesInterfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Dumps.Infrastructure.Services;

public class PaymentService: IPaymentService
{
    private readonly string _stripeSecretKey;

    public PaymentService(IConfiguration configuration)
    {
        _stripeSecretKey = configuration["Stripe:SecretKey"];
        StripeConfiguration.ApiKey = _stripeSecretKey;
    }

    public async Task<string> CreatePaymentIntent(long amount)
    {
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amount, // Amount in cents
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "card" },
        });

        return paymentIntent.ClientSecret;
    }
}
