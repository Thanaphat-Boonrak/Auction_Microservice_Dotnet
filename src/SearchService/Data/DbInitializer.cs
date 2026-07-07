using System.Text.Json;
using MongoDB.Entities;
using SearchService.Model;
using SearchService.Services;

namespace AuctionService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.Default.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.Default.CountAsync<Item>();
        
        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await httpClient.GetItemForSearchDb();
         
         
        if(count == 0) await DB.Default.SaveAsync(items);
    }
}