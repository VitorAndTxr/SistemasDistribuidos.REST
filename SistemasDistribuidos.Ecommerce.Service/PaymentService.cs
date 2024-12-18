using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;
using System.Net.Http;
using System.Text;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class PaymentService : RabbitMQTopicService
    {
        private readonly IConfiguration _configuration;
        public PaymentService(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task HandleListenCreatedBuyRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            ValidatePayload(payload);

            if (payload.TotalPrice < 1000)
            {
                payload.Status = Domain.Enum.BuyRequestStatusEnum.PaymentApproved;
                //await Publish("ApprovedPaymentQueueName", JsonConvert.SerializeObject(payload));

                using var httpClient = new HttpClient();

                await httpClient.PostAsync(payload.WebhookURL, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
            }
            else
            {
                payload.Status = Domain.Enum.BuyRequestStatusEnum.PaymentDenied;
                //await Publish("DeniedPaymentQueueName", JsonConvert.SerializeObject(payload));

                using var httpClient = new HttpClient();

                await httpClient.PostAsync(payload.WebhookURL, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

            }
        }

        private static void ValidatePayload(BuyRequestViewModel? payload)
        {
            if (payload == null)
                throw new Exception("Invalid payload");

            if (payload.Products == null || payload.Products.Count == 0)
                throw new Exception("Invalid payload");
        }

        public async Task ListenCreatedBuyRequest()
        {
            await CreateListener("CreatedBuyRequestQueueName", async (model, ea) =>
            {
                HandleListenCreatedBuyRequest(model, ea).Wait();
            });
        }
    }
}
