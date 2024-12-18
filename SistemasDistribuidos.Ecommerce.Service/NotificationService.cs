using Microsoft.Extensions.Configuration;
using System.Text;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class NotificationService: RabbitMQTopicService
    {
        private readonly IConfiguration _configuration;
        public NotificationService(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
        }


        public async Task HandleListenBuyRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            var notification = $"Compra {payload.Id} no valor de {payload.TotalPrice} está aguardando a aprovação do pagamento!";

            SseMessageChannel.MessageChannel.Writer.TryWrite(notification);

            var routingKey = ea.RoutingKey;

            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
        }

        public async Task HandleListenApprovedPaymentEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            var notification = $"Compra {payload.Id} no valor de {payload.TotalPrice} teve o pagamento aprovado!";

            SseMessageChannel.MessageChannel.Writer.TryWrite(notification);

            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
        }

        public async Task HandleListenRepprovedPaymentEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            var notification = $"Compra {payload.Id} no valor de {payload.TotalPrice} teve o pagamento recusado!";

            SseMessageChannel.MessageChannel.Writer.TryWrite(notification);

            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

        }

        public async Task HandleListenShippedRequestEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            var notification = $"Compra {payload.Id} foi enviada!";

            SseMessageChannel.MessageChannel.Writer.TryWrite(notification);

            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
        }

        #region Listeners

        public async Task ListenCreatedBuyRequest()
        {
            await CreateListener("CreatedBuyRequestQueueName", async (model, ea) =>
            {
                HandleListenBuyRequest(model,ea).Wait();
            });
        }

        public async Task ListenApprovedPaymentEvent()
        {
            await CreateListener("ApprovedPaymentQueueName", async (model, ea) =>
            {
                HandleListenApprovedPaymentEvent(model, ea).Wait();
            });
        }

        public async Task ListenRepprovedPaymentEvent()
        {
            await CreateListener("DeniedPaymentQueueName", async (model, ea) =>
            {
                HandleListenRepprovedPaymentEvent(model, ea).Wait();
            });
        }

        public async Task ListenShippedRequestEvent()
        {
            await CreateListener("ShippedRequestQueueName", async (model, ea) =>
            {
                HandleListenShippedRequestEvent(model, ea).Wait();
            });
        }

        #endregion
    }
}
