using System.Globalization;
using System.Text;
using Duende.IdentityServer.Licensing;
using IdentityService;
using Npgsql;
using Polly;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var app = builder
        .ConfigureLogging()
        .ConfigureServices()
        .ConfigurePipeline();

    // this seeding is only for the template to bootstrap the DB and users.
    // in production you will likely want a different approach.
    var retryPolicy = Policy.Handle<NpgsqlException>()
        .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    retryPolicy.ExecuteAndCapture(() => SeedData.EnsureSeedData(app));


    if (app.Environment.IsDevelopment())
    {
        _ = app.Lifetime.ApplicationStopping.Register(() =>
        {
            var usage = app.Services.GetRequiredService<LicenseUsageSummary>();
            Console.Write(Summary(usage));
        });
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

static string Summary(LicenseUsageSummary usage)
{
    var sb = new StringBuilder();
    _ = sb.AppendLine("IdentityServer Usage Summary:");
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  License: {string.Join(", ", usage.EntitledSkus)}");
    var features = usage.FeaturesUsed.Count > 0 ? string.Join(", ", usage.FeaturesUsed) : "None";
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  Business and Enterprise Edition Features Used: {features}");
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.ClientsUsed.Count} Client Id(s) Used");
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.IssuersUsed.Count} Issuer(s) Used");

    return sb.ToString();
}