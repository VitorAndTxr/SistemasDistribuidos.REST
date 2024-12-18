using Microsoft.Extensions.Configuration;
using System.Text;
using RabbitMQ.Client.Events;
using SistemasDistribuidos.Ecommerce.Domain.Entity;
using SistemasDistribuidos.Ecommerce.Service.Interface;
using System.Net.Http.Json;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;
using SistemasDistribuidos.Ecommerce.Domain.Enum;
using Newtonsoft.Json;
using SistemasDistribuidos.Ecommerce.Domain.Payload;

namespace SistemasDistribuidos.Ecommerce.Service
{
    public class EcomerceService:RabbitMQTopicService, IEcommerceService
    {
        private readonly IConfiguration _configuration;

        private readonly string _stockServiceUrl = "http://localhost:5005";

        private readonly string _buyRequestsFilePath = "buyRequests.json";

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

        public async void HandleCreateBuyRequest(BuyRequestPayload payload)
        {
            try
            {
                ValidateBuyRequest(payload);

                var id = Guid.NewGuid();

                BuyRequestViewModel buyRequest = new BuyRequestViewModel
                {
                    Id = id,
                    UserId = payload.UserId,
                    WebhookURL=$"https://localhost:5000/payment/{id}",
                    Status = BuyRequestStatusEnum.PendingPayment,
                    Products = payload.Items,
                    TotalPrice = payload.Items.Sum(x => x.Price * x.Stock)
                
                };

                SaveBuyRequest(buyRequest);

                await Publish("CreatedBuyRequestQueueName", JsonConvert.SerializeObject(buyRequest));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void HandleDeleteBuyRequest(Guid buyRequestId)
        {
            try
            {

                BuyRequestViewModel buyRequest = GetBuyRequestById(buyRequestId);

                if(buyRequest == null)
                {
                    throw new Exception($"Buy request with ID {buyRequestId} not found");
                }

                SaveBuyRequest(buyRequest);

                await Publish("DeletedBuyRequestQueueName", JsonConvert.SerializeObject(buyRequest));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void HandlePaymentRequestResponse(BuyRequestViewModel payload)
        {
            try
            {
                RemoveBuyRequest(payload.Id);

                SaveBuyRequest(payload);

                if(payload.Status == BuyRequestStatusEnum.PaymentApproved)
                {
                    await Publish("ApprovedPaymentQueueName", JsonConvert.SerializeObject(payload));
                }
                else if(payload.Status == BuyRequestStatusEnum.PaymentDenied)
                {
                    await Publish("DeniedPaymentQueueName", JsonConvert.SerializeObject(payload));
                }
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
                // Check if the file exists
                if (File.Exists(_buyRequestsFilePath))
                {
                    // Read the existing JSON data from the file
                    string existingJson = File.ReadAllText(_buyRequestsFilePath);

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

        public BuyRequestViewModel GetBuyRequestById(Guid id)
        {

            try
            {
                // Check if the file exists
                if (File.Exists(_buyRequestsFilePath))
                {
                    // Read the existing JSON data from the file
                    string existingJson = File.ReadAllText(_buyRequestsFilePath);

                    // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                    List<BuyRequestViewModel> existingBuyRequests = JsonConvert.DeserializeObject<List<BuyRequestViewModel>>(existingJson);

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
        private void SaveBuyRequest(BuyRequestViewModel buyRequest)
        {
            string json = JsonConvert.SerializeObject(buyRequest);


            // Check if the file exists
            if (File.Exists(_buyRequestsFilePath))
            {
                // Read the existing JSON data from the file
                string existingJson = File.ReadAllText(_buyRequestsFilePath);

                // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                List<BuyRequestViewModel> existingBuyRequests = JsonConvert.DeserializeObject<List<BuyRequestViewModel>>(existingJson);

                // Add the new buyRequest to the existing list
                existingBuyRequests.Add(buyRequest);

                // Convert the updated list back to JSON
                string updatedJson = JsonConvert.SerializeObject(existingBuyRequests);

                // Write the updated JSON data to the file
                File.WriteAllText(_buyRequestsFilePath, updatedJson);
            }
            else
            {
                // Create a new list with the buyRequest as the only item
                List<BuyRequestViewModel> buyRequests = new List<BuyRequestViewModel> { buyRequest };

                // Convert the list to JSON
                string newJson = JsonConvert.SerializeObject(buyRequests);

                // Write the JSON data to the file
                File.WriteAllText(_buyRequestsFilePath, newJson);
            }
        }

        private void RemoveBuyRequest(Guid id)
        {
            try
            {
                // Check if the file exists
                if (File.Exists(_buyRequestsFilePath))
                {
                    // Read the existing JSON data from the file
                    string existingJson = File.ReadAllText(_buyRequestsFilePath);

                    // Deserialize the existing JSON data into a list of BuyRequesViewModel objects
                    List<BuyRequestViewModel> existingBuyRequests = JsonConvert.DeserializeObject<List<BuyRequestViewModel>>(existingJson);

                    // Find the buy request with the specified ID
                    BuyRequestViewModel buyRequestToRemove = existingBuyRequests.FirstOrDefault(x => x.Id == id);

                    // Check if the buy request exists
                    if (buyRequestToRemove != null)
                    {
                        // Remove the buy request from the list
                        existingBuyRequests.Remove(buyRequestToRemove);

                        // Convert the updated list back to JSON
                        string updatedJson = JsonConvert.SerializeObject(existingBuyRequests);

                        // Write the updated JSON data to the file
                        File.WriteAllText(_buyRequestsFilePath, updatedJson);
                    }
                }
            }
            catch (Exception)
            {
                throw;
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
}
