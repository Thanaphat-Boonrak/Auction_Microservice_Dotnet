using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<List<Item>> GetItemForSearchDb()
    {
        var lastUpdated = await DB.Default.Find<Item, string>()
            .Sort(x => x.Descending(x => x.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();
        
        return await httpClient.GetFromJsonAsync<List<Item>>($"{configuration["AuctionServiceUrl"]}/api/auctions?date={lastUpdated}");
    }
}