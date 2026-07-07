using MassTransit;
using NotificationService.Cosumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMassTransit(config =>
{

    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification"));
    
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/" , h =>
        {
            h.Username(builder.Configuration.GetValue<string>("RabbitMQ:Username","guest"));
            h.Password(builder.Configuration.GetValue<string>("RabbitMQ:Password","guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddSignalR();


var app = builder.Build();

app.MapHub<NotificationHub>("/notification");

app.Run();
