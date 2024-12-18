using SistemasDistribuidos.Ecommerce.Service;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var service = new PaymentService(configuration);

await service.ListenCreatedBuyRequest();

while (true)
{
    await Task.Delay(1000);
}