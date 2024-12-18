using SistemasDistribuidos.Ecommerce.Service;


var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) // Define o diretório base onde está o appsettings.json
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true) // Carrega o arquivo appsettings.json
    .Build();

ShipmentService shipmentService = new ShipmentService(config);

shipmentService.ListenApprovedPaymentEvent();

while (true)
{
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
    break;
}