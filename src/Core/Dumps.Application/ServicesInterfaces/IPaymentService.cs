using Microsoft.AspNetCore.Http;

namespace Dumps.Application.ServicesInterfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntent(long amount, string userId);
        Task HandleStripeWebhookAsync(string json, string stripeSignature);
    }
}
