using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Service.Interface;
using System.Text;
using RabbitMQ.Client.Events;
using System.Net.Http.Json;
using Newtonsoft.Json;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class StockService: RabbitMQTopicService,IStockService
    {
        private readonly IConfiguration _configuration;
        private readonly static string _stockFilePath = "stock.json";
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
            try
            {
                if (File.Exists(_stockFilePath))
                {
                    // Read the existing JSON data from the file
                    string existingJson = File.ReadAllText(_stockFilePath);

                    // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                    List<Product> stock = JsonConvert.DeserializeObject<List<Product>>(existingJson);

                    // Add the new buyRequest to the existing list
                    return stock;
                }
                else
                {
                    var products = new List<Product>
                {
                    new Product {
                        Id = Guid.NewGuid(),
                        Name = "Camisa 1",
                        Description = "Camisa de Algodão",
                        Image = "https://www.google.com.br",
                        Price = 50,
                        Stock = 10
                    },
                    new Product {
                        Id = Guid.NewGuid(),
                        Name = "Calça 1",
                        Description = "Camisa de Algodão",
                        Image = "https://www.google.com.br",
                        Price = 70,
                        Stock = 10
                    },
                    new Product {
                        Id = Guid.NewGuid(),
                        Name = "Blusa 1",
                        Description = "Camisa de Algodão",
                        Image = "https://www.google.com.br",
                        Price = 100,
                        Stock = 10
                    },
                    new Product {
                        Id = Guid.NewGuid(),
                        Name = "Jaqueta 1",
                        Description = "Camisa de Algodão",
                        Image = "https://www.google.com.br",
                        Price = 500,
                        Stock = 10
                    },
                    new Product {
                        Id = Guid.NewGuid(),
                        Name = "meia 1",
                        Description = "Camisa de Algodão",
                        Image = "https://www.google.com.br",
                        Price = 5,
                        Stock = 10
                    }
                };

                    // Convert the list to JSON
                    string newJson = JsonConvert.SerializeObject(products);

                    // Write the JSON data to the file
                    File.WriteAllText(_stockFilePath, newJson);

                    return products;
                }
            }
            catch (Exception ex)
            {
                throw;
            }   
        }
        #endregion
    }
}
