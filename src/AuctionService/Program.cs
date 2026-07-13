using System.Text.Json.Serialization;
using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.RequestHelpers;
using AuctionService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddMassTransit(config =>
{
    config.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    config.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction"));

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue<string>("RabbitMQ:Username", "guest"));
            h.Password(builder.Configuration.GetValue<string>("RabbitMQ:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServiceUrl"];
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters.ValidateAudience = false;
    opt.TokenValidationParameters.NameClaimType = "username";
});
builder.Services.AddGrpc();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<GrpcAuctionService>();

var retryPolicy = Policy.Handle<NpgsqlException>()
    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
await retryPolicy.ExecuteAndCaptureAsync(async () => await DbInitializer.InitDb(app));

app.Run();

public partial class Program
{
}