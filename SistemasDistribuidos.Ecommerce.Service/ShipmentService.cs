using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using SistemasDistribuidos.Ecommerce.Domain.Enum;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;
using System.Text;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class ShipmentService : RabbitMQTopicService
    {
        private readonly IConfiguration _configuration;
        public ShipmentService(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task HandleListenApprovedPaymentEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            payload.Status = BuyRequestStatusEnum.Shipped;

            await Publish("ShippedRequestQueueName", JsonConvert.SerializeObject(payload));
        }

        #region Listeners
        public async Task ListenApprovedPaymentEvent()
        {
            await CreateListener("ApprovedPaymentQueueName", async (model, ea) =>
            {
                HandleListenApprovedPaymentEvent(model, ea).Wait();
            });
        }

        #endregion
    }
}
