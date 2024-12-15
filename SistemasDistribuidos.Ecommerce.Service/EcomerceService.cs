using Microsoft.Extensions.Configuration;
using System.Text;
using RabbitMQ.Client.Events;
using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Service.Interface;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class EcomerceService:RabbitMQTopicService, IEcomerceService
    {
        private readonly IConfiguration _configuration;
        public EcomerceService(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
        }

        public List<Product> GetProductsInStock()
        {
            throw new NotImplementedException();
            //return stock.Where(x => x.Stock > 0).ToList();
        }

        public async Task HandleListenShippedRequestEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            throw new NotImplementedException();
        }

        public async Task ListenShippedRequestEvent(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenShippedRequestEvent(model, ea).Wait();
            });
        }
    }

    public class CartService: ICartService
    {
        public CartService()
        {
            
        }

    }
}
