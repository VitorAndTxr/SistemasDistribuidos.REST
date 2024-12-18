using Newtonsoft.Json;
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
        .WithName("Buscar Produtos Disponíveis")
        .WithOpenApi();

        #endregion

        #region Carrinho

        app.MapGet("/carrinho", () =>
        {

        }).
        WithName("Buscar Carrinho")
        .WithOpenApi();

        app.MapPut("/carrinho", () =>
        {

        }).
        WithName("Atualizar Carrinho")
        .WithOpenApi();

        app.MapPost("/carrinho", () =>
        {

        }).
        WithName("Adicionar Produto ao Carrinho")
        .WithOpenApi();

        app.MapDelete("/carrinho", () =>
        {

        }).
        WithName("Remover Produto do Carrinho")
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
                // Agora você tem o conteúdo do Stream como uma string na variável "content"
                var payload = JsonConvert.DeserializeObject<BuyRequestPayload>(content);

                ecomerceService.HandleBuyRequest(payload);
            }
            return Results.Ok();

        }).
        WithName("Realizar Pedido")
        .Accepts(typeof(BuyRequestPayload), "application/json")
        .WithOpenApi();



        #endregion

        app.Run();
    }
}
