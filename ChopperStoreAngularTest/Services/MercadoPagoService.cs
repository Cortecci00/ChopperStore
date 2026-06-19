using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using ChopperStoreAngularTest.Models;

namespace ChopperStoreAngularTest.Services
{
    public interface IMercadoPagoService
    {
        Task<(string PreferenceId, string InitPoint)> CreatePreferenceAsync(Transaction transaction, List<Item> cartItems);
        Task<Payment> GetPaymentAsync(long paymentId);
    }

    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;

        public MercadoPagoService(IConfiguration configuration)
        {
            _configuration = configuration;
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];
        }

        public async Task<(string PreferenceId, string InitPoint)> CreatePreferenceAsync(Transaction transaction, List<Item> cartItems)
        {
            var backendUrl = _configuration["App:PublicBackendUrl"];
            var frontendUrl = _configuration["App:FrontendUrl"];

            var request = new PreferenceRequest
            {
                Items = cartItems.Select(i => new PreferenceItemRequest
                {
                    Title = i.skin.name,
                    Quantity = i.quantity,
                    CurrencyId = "ARS",
                    UnitPrice = (decimal)i.skin.price
                }).ToList(),
                ExternalReference = transaction.Id.ToString(),
                NotificationUrl = $"{backendUrl}/api/Transaction/webhook/mercadopago",
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = $"{frontendUrl}/payment-return?status=success",
                    Pending = $"{frontendUrl}/payment-return?status=pending",
                    Failure = $"{frontendUrl}/payment-return?status=failure"
                },
                AutoReturn = "approved"
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(request);

            return (preference.Id, preference.InitPoint);
        }

        public async Task<Payment> GetPaymentAsync(long paymentId)
        {
            var client = new PaymentClient();
            return await client.GetAsync(paymentId);
        }
    }
}
