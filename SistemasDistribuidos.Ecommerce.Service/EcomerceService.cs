using Microsoft.Extensions.Configuration;
using System.Text;
using RabbitMQ.Client.Events;
using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Service.Interface;
using System.Net.Http.Json;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;
using SistemasDistribuidos.Ecommerce.Domain.Enum;
using Newtonsoft.Json;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class EcomerceService:RabbitMQTopicService, IEcommerceService
    {
        private readonly IConfiguration _configuration;

        private readonly string _stockServiceUrl = "http://localhost:5005";
        public EcomerceService(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
        }

        public List<Product> GetProductsInStock()
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    // Specify the URL of the HTTP endpoint
                    string url = _stockServiceUrl + "/ListProductsInStock";

                    // Send a GET request to the endpoint and retrieve the response
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Deserialize the response body into a list of Product objects
                        List<Product> products = response.Content.ReadFromJsonAsync<List<Product>>().Result;

                        // Filter the products to only include those in stock
                        List<Product> productsInStock = products.Where(x => x.Stock > 0).ToList();

                        return productsInStock;
                    }
                    else
                    {
                        // Handle the case when the request was not successful
                        throw new Exception($"Failed to retrieve products. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ...

        public void HandleBuyRequest(BuyRequestPayload payload)
        {
            try
            {
                ValidateBuyRequest(payload);

                BuyRequestViewModel buyRequest = new BuyRequestViewModel
                {
                    Id = Guid.NewGuid(),
                    UserId = payload.UserId,
                    Status = BuyRequestStatusEnum.Pending,
                    Products = payload.Items
                };

                // Convert the buyRequest object to JSON
                SaveBuyRequest(buyRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BuyRequestViewModel> GetUserBuyRequestList(string userId)
        {

            try
            {
                // Specify the path to the JSON file
                string filePath = "buyRequests.json";

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the existing JSON data from the file
                    string existingJson = File.ReadAllText(filePath);

                    // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                    List<BuyRequestViewModel> existingBuyRequests = JsonConvert.DeserializeObject<List<BuyRequestViewModel>>(existingJson);

                    // Filter the buy requests to only include those for the specified user
                    List<BuyRequestViewModel> userBuyRequests = existingBuyRequests.Where(x => x.UserId == userId).ToList();

                    return userBuyRequests;
                }
                else
                {
                    // Handle the case when the file does not exist
                    return new List<BuyRequestViewModel>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void SaveBuyRequest(BuyRequestViewModel buyRequest)
        {
            string json = JsonConvert.SerializeObject(buyRequest);

            // Specify the path to the JSON file
            string filePath = "buyRequests.json";

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the existing JSON data from the file
                string existingJson = File.ReadAllText(filePath);

                // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                List<BuyRequestViewModel> existingBuyRequests = JsonConvert.DeserializeObject<List<BuyRequestViewModel>>(existingJson);

                // Add the new buyRequest to the existing list
                existingBuyRequests.Add(buyRequest);

                // Convert the updated list back to JSON
                string updatedJson = JsonConvert.SerializeObject(existingBuyRequests);

                // Write the updated JSON data to the file
                File.WriteAllText(filePath, updatedJson);
            }
            else
            {
                // Create a new list with the buyRequest as the only item
                List<BuyRequestViewModel> buyRequests = new List<BuyRequestViewModel> { buyRequest };

                // Convert the list to JSON
                string newJson = JsonConvert.SerializeObject(buyRequests);

                // Write the JSON data to the file
                File.WriteAllText(filePath, newJson);
            }
        }

        private void ValidateBuyRequest(BuyRequestPayload payload)
        {
            var productsInStock = GetProductsInStock();

            foreach (var productToBuy in payload.Items)
            {
                var productInStock = productsInStock.FirstOrDefault(x => x.Id == productToBuy.Id);

                if (productInStock == null)
                {
                    throw new Exception($"Product with ID {productToBuy.Id} not found in stock");
                }

                if (productToBuy.Stock > productInStock.Stock)
                {
                    throw new Exception($"Not enough stock for product with ID {productToBuy.Id}");
                }
            }
        }

        public async Task ListenShippedRequestEvent(string topicNameConfig)
        {
            await CreateListener(topicNameConfig, async (model, ea) =>
            {
                HandleListenShippedRequestEvent(model, ea).Wait();
            });
        }

        public async Task HandleListenShippedRequestEvent(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
            throw new NotImplementedException();
        }
    }

    public class CartService: ICartService
    {
        public CartService()
        {
            
        }

        public Guid CreateCart()
        {
            throw new NotImplementedException();
        }

    }
}
