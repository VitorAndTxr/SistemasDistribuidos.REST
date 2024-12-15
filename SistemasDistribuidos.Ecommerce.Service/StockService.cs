using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Service.Interface;
using System.Text;
using RabbitMQ.Client.Events;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class StockService: RabbitMQTopicService,IStockService
    {
        private readonly IConfiguration _configuration;
        private List<Product> stock = InitStock();

        public StockService(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
        }

        public List<Product> GetProductsInStock()
        {
            return stock.Where(x => x.Stock > 0).ToList();
        }
        public async Task HandleListenCreatedBuyRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            throw new NotImplementedException();
        }

        public async Task HandleListenDeletedBuyRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            throw new NotImplementedException();
        }

        #region BuyRequestListeners

        public async Task ListenCreatedBuyRequest(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenCreatedBuyRequest(model, ea).Wait();
            });
        }
        public async Task ListenDeletedBuyRequest(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenDeletedBuyRequest(model, ea).Wait();
            });
        }

        #endregion

        #region InitStock
        private static List<Product> InitStock()
        {
            return new List<Product>
            {
                new Product { 
                    Id = Guid.NewGuid(), 
                    Name = "Camisa",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 50,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "Calça",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 70,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "Blusa",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 100,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "Jaqueta",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 500,
                    Stock = 10
                },
                new Product {
                    Id = Guid.NewGuid(),
                    Name = "meia",
                    Description = "Camisa de Algodão",
                    Image = "https://www.google.com.br",
                    Price = 5,
                    Stock = 10
                }
            };
        }
        #endregion
    }

    public class PaymentService
    {
        private readonly IConfiguration _configuration;
        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
