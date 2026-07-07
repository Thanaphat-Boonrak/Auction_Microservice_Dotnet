using System.Net;
using AuctionService.Data;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Model;
using SearchService.RequestHelper;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(config =>
{
    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search"));
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"],"/", host =>
        {
            host.Username(builder.Configuration.GetValue<string>("RabbitMQ:Username" ,"guest"));
            host.Password(builder.Configuration.GetValue<string>("RabbitMQ:Password","guest"));
        });
        
        cfg.ReceiveEndpoint("search-auction-created",
            e =>
            {
                e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                e.UseMessageRetry(r => r.Interval(5, 5000));
            });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();


app.UseAuthorization();

app.MapControllers();


var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
var db = await DB.InitAsync("SearchDatabase", MongoClientSettings.FromConnectionString(connectionString));
await DbInitializer.InitDb(app);

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetPolicy() => HttpPolicyExtensions.HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(5));