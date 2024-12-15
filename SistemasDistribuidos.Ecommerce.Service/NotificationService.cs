using Microsoft.Extensions.Configuration;
using System.Text;
using RabbitMQ.Client.Events;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class NotificationService: RabbitMQTopicService
    {
        private readonly IConfiguration _configuration;
        public NotificationService(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
        }


        public async Task HandleListenByRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

            throw new NotImplementedException();
        }

        public async Task HandleListenApprovedPaymentEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

            throw new NotImplementedException();
        }

        public async Task HandleListenRepprovedPaymentEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            throw new NotImplementedException();
        }

        public async Task HandleListenShippedRequestEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            throw new NotImplementedException();
        }

        #region Listeners

        public async Task ListenCreatedBuyRequest(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenByRequest(model,ea).Wait();
            });
        }

        public async Task ListenApprovedPaymentEvent(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenApprovedPaymentEvent(model, ea).Wait();
            });
        }

        public async Task ListenRepprovedPaymentEvent(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenRepprovedPaymentEvent(model, ea).Wait();
            });
        }

        public async Task ListenShippedRequestEvent(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenShippedRequestEvent(model, ea).Wait();
            });
        }

        #endregion
    }
}
