using SistemasDistribuidos.Ecommerce.Service;
using SistemasDistribuidos.Ecommerce.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the StockService as a dependency
builder.Services.AddSingleton<IStockService, StockService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ListProductsInStock", (IStockService stockService) =>
{
    var productsInStock = stockService.GetProductsInStock();
    return Results.Ok(productsInStock);
})
.WithName("ListProductsInStock")
.WithOpenApi();

app.Run();
