var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Produtos

app.MapGet("/produtos", () =>
{

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

app.MapGet("/pedido", () =>
{

}).
WithName("Buscar Pedido")
.WithOpenApi();

app.MapPost("/pedido", () =>
{

}).
WithName("Realizar Pedido")
.WithOpenApi();

#endregion

app.Run();
