using SistemasDistribuidos.Ecommerce.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
app.UseCors("AllowAll");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
var notificationService = new NotificationService(configuration);


notificationService.ListenCreatedBuyRequest();
notificationService.ListenApprovedPaymentEvent();
notificationService.ListenRepprovedPaymentEvent();

notificationService.ListenShippedRequestEvent();



app.MapGet("/sse", async (HttpContext context) =>
{
    context.Response.Headers.Add("Cache-Control", "no-cache");
    context.Response.Headers.Add("Content-Type", "text/event-stream");

    var reader = SseMessageChannel.MessageChannel.Reader;

    // Ler do canal até a conexão ser interrompida ou o canal ser completo.
    while (!context.RequestAborted.IsCancellationRequested)
    {
        if (await reader.WaitToReadAsync(context.RequestAborted))
        {
            while (reader.TryRead(out var message))
            {
                Console.WriteLine($"Sending message: {message}");
                await context.Response.WriteAsync($"data: {message}\n\n");
                await context.Response.Body.FlushAsync();
            }
        }
    }
});

app.Run();

