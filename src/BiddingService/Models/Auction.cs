using MongoDB.Entities;

namespace BiddingService.Models;

public class Auction : Entity
{
    public DateTime AuctionEnd { get; set; }
    
    public string Seller { get; set; } = string.Empty;
    
    public int ReservedPrice { get; set; }
    public bool Finished { get; set; }
}