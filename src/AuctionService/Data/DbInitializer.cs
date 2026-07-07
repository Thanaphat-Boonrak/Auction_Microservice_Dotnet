using System.Text.Json;
using System.Text.Json.Serialization;
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        await SeedData(scope.ServiceProvider.GetRequiredService<AuctionDbContext>());
    }

    private static async Task SeedData(AuctionDbContext context)
    {
        await context.Database.MigrateAsync();

        if (context.Auctions.Any())
        {
            Console.WriteLine("Already Seeded!");
            return;
        }
        
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data","SeedData", "auctionSeedData.json");
        var auctions = JsonSerializer.Deserialize<List<Auction>>(await File.ReadAllTextAsync(filePath),options);
        if(auctions == null) return;
        context.AddRange(auctions);
        await context.SaveChangesAsync();
    }
}