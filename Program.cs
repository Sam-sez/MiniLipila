using FastEndpoints;
using FastEndpoints.Swagger;
using MiniLipila.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDisbursementStore, InMemoryDisbursementStore>();
builder.Services.AddSingleton<ITransactionStore, InMemoryTransactionStore>();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(); // enables Swagger for our endpoints
builder.Services.AddHttpClient("Lipila", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["Lipila:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("x-api-key", config["Lipila:ApiKey"]);
    client.DefaultRequestHeaders.Add("accept", "application/json");
});
var app = builder.Build();

app.UseFastEndpoints();
app.UseSwaggerGen(); // serves the Swagger UI page

app.Run();