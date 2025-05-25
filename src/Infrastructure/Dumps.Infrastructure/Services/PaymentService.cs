using Dumps.Application.ServicesInterfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Dumps.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly string _stripeSecretKey;

    public PaymentService(IConfiguration configuration)
    {
        _stripeSecretKey = configuration["Stripe:SecretKey"];
        StripeConfiguration.ApiKey = _stripeSecretKey;
    }

    public async Task<string> CreatePaymentIntent(long amount, string userId)
    {
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amount, // Amount in cents
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "card" },
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId }
            }
        });

        return paymentIntent.ClientSecret;
    }

    public async Task HandleStripeWebhookAsync(string json, string stripeSignature)
    {
        var endpointSecret = "whsec_22e0607033e4821282711fc7ec1741c1bb74beaf70cd6272805dc59894300ef4"; // Ideally a separate webhook secret from config

        var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                // TODO: Handle success (e.g., update order status in DB)
                var userId = succeededIntent.Metadata["userId"];
                Console.WriteLine($"Payment succeeded: {succeededIntent?.Id}");
                break;

            case "payment_intent.payment_failed":
                var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                // TODO: Handle failure (e.g., notify user)
                Console.WriteLine($"Payment failed: {failedIntent?.Id}");
                break;

            default:
                Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                break;
        }
    }
}
