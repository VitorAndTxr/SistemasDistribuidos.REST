using Newtonsoft.Json;
using SistemasDistribuidos.Ecommerce.Domain.Payload;
using SistemasDistribuidos.Ecommerce.Domain.ViewModel;
using SistemasDistribuidos.Ecommerce.Service;
using SistemasDistribuidos.Ecommerce.Service.Interface;
using System.IO;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IEcommerceService, EcomerceService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Apply the CORS policy
        app.UseCors("AllowAll");

        #region Produtos

        app.MapGet("/produtos", (IEcommerceService ecomerceService) =>
        {
            var productsInStock = ecomerceService.GetProductsInStock();
            return Results.Ok(productsInStock);
        })
        .WithName("Buscar Produtos Dispon�veis")
        .WithOpenApi();

        #endregion

        #region Pedido

        app.MapGet("/pedidos/{userId}", (IEcommerceService ecomerceService, string userId) =>
        {
            return Results.Ok(ecomerceService.GetUserBuyRequestList(userId)); 
        }).
        WithName("Buscar Pedido")
        .WithOpenApi();

        app.MapPost("/buy", (IEcommerceService ecomerceService, HttpRequest request) =>
        {
            var requestBody = request.Body;

            using (StreamReader reader = new StreamReader(requestBody))
            {
                string content = reader.ReadToEndAsync().Result;
                // Agora voc� tem o conte�do do Stream como uma string na vari�vel "content"
                var payload = JsonConvert.DeserializeObject<BuyRequestPayload>(content);

                ecomerceService.HandleCreateBuyRequest(payload);
            }
            return Results.Ok();

        }).
        WithName("Realizar Pedido")
        .Accepts(typeof(BuyRequestPayload), "application/json")
        .WithOpenApi();

        app.MapDelete("/buy/{id}", (IEcommerceService ecomerceService, Guid id) =>
        {
            ecomerceService.HandleDeleteBuyRequest(id);
            return Results.Ok();
        })
        .WithName("Deletar Pedido")
        .WithOpenApi();

        app.MapPost("payment/{id}", (IEcommerceService ecomerceService, Guid id , HttpRequest request) =>
        {
            var requestBody = request.Body;

            using (StreamReader reader = new StreamReader(requestBody))
            {
                string content = reader.ReadToEndAsync().Result;
                // Agora voc� tem o conte�do do Stream como uma string na vari�vel "content"
                var payload = JsonConvert.DeserializeObject<BuyRequestViewModel>(content);

                ecomerceService.HandlePaymentRequestResponse(payload);
            }
            return Results.Ok();

        })
        .WithName("Confirmar pagamentos")
        .Accepts(typeof(BuyRequestPayload), "application/json")
        .WithOpenApi();



        #endregion

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var service = new EcomerceService(configuration);

        service.ListenShippedRequestEvent();
        app.Run();
    }
}
