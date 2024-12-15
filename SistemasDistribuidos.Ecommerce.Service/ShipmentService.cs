using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
            throw new NotImplementedException();
        }

        #region Listeners
        public async Task ListenApprovedPaymentEvent(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenApprovedPaymentEvent(model, ea).Wait();
            });
        }

        #endregion
    }
}
