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

        public StockService(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
        }

        public List<Product> GetProductsInStock()
        {
            var stock = InitStock();
            return stock.Where(x => x.Stock > 0).ToList();
        }
        public async Task HandleListenCreatedBuyRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            foreach(var item in payload.Products)
            {
                RemoveProductInStock(item);
            }

            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
        }

        public async Task HandleListenDeletedBuyRequest(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(message);

            foreach (var item in payload.Products)
            {
                AddProductInStock(item);
            }
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
        }

        #region BuyRequestListeners

        public async Task ListenCreatedBuyRequest()
        {
            await CreateListener("CreatedBuyRequestQueueName", async (model, ea) =>
            {
                HandleListenCreatedBuyRequest(model, ea).Wait();
            });
        }
        public async Task ListenDeletedBuyRequest()
        {
            await CreateListener("DeletedBuyRequestQueueName", async (model, ea) =>
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

        public ProductViewModel GetProductById(Guid id)
        {

            try
            {
                // Check if the file exists
                if (File.Exists(_stockFilePath))
                {
                    // Read the existing JSON data from the file
                    string existingJson = File.ReadAllText(_stockFilePath);

                    // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                    List<ProductViewModel> existingBuyRequests = JsonConvert.DeserializeObject<List<ProductViewModel>>(existingJson);

                    // Filter the buy requests to only include those for the specified user

                    return existingBuyRequests.FirstOrDefault(x => x.Id == id);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void RemoveProductInStock(ProductViewModel productToChange)
        {
            string json = JsonConvert.SerializeObject(productToChange);


            // Check if the file exists
            if (File.Exists(_stockFilePath))
            {
                // Read the existing JSON data from the file
                string existingJson = File.ReadAllText(_stockFilePath);

                // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                List<ProductViewModel> productInStock = JsonConvert.DeserializeObject<List<ProductViewModel>>(existingJson);

                foreach (var item in productInStock)
                {
                    if (item.Id == productToChange.Id)
                    {
                        item.Stock -= productToChange.Stock;
                        break;
                    }
                }

                // Convert the updated list back to JSON
                string updatedJson = JsonConvert.SerializeObject(productInStock);

                // Write the updated JSON data to the file
                File.WriteAllText(_stockFilePath, updatedJson);
            }
        }

        private void AddProductInStock(ProductViewModel productToChange)
        {
            string json = JsonConvert.SerializeObject(productToChange);


            // Check if the file exists
            if (File.Exists(_stockFilePath))
            {
                // Read the existing JSON data from the file
                string existingJson = File.ReadAllText(_stockFilePath);

                // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                List<ProductViewModel> productInStock = JsonConvert.DeserializeObject<List<ProductViewModel>>(existingJson);

                bool productExists = false;

                foreach (var item in productInStock)
                {
                    if (item.Id == productToChange.Id)
                    {
                        item.Stock += productToChange.Stock;
                        productExists = true;
                        break;
                    }
                }

                if(!productExists)
                {
                    productInStock.Add(productToChange);
                }

                // Convert the updated list back to JSON
                string updatedJson = JsonConvert.SerializeObject(productInStock);

                // Write the updated JSON data to the file
                File.WriteAllText(_stockFilePath, updatedJson);
            }
        }

        #endregion
    }
}
