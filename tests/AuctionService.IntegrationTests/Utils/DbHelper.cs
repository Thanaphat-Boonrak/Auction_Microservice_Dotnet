using AuctionService.Data;
using AuctionService.Entities;

namespace AuctionService.IntegrationTests.Utils;

public class DbHelper
{
    public static void InitDbForTests(AuctionDbContext db)
    {
        db.Auctions.AddRange(GetAuctionForTest());
        db.SaveChanges();
    }

    public static void ReintialDbForTests(AuctionDbContext db)
    {
        db.Auctions.RemoveRange(GetAuctionForTest());
        db.SaveChanges();
        InitDbForTests(db);
    }

    private static List<Auction> GetAuctionForTest()
    {
        return new List<Auction>
        {
            new Auction
            {
                Id = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "bob",
                AuctionEnd = DateTime.Parse("2026-06-29T00:08:41Z").ToUniversalTime(),
                Item = new Item
                {
                    Make = "Ford",
                    Model = "GT",
                    Color = "White",
                    Mileage = 50000,
                    Year = 2020,
                    ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg"
                }
            },
            new Auction
            {
                Id = Guid.Parse("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"),
                Status = Status.Live,
                ReservePrice = 90000,
                Seller = "alice",
                AuctionEnd = DateTime.Parse("2026-08-18T00:08:41Z").ToUniversalTime(),
                Item = new Item
                {
                    Make = "Bugatti",
                    Model = "Veyron",
                    Color = "Black",
                    Mileage = 15035,
                    Year = 2018,
                    ImageUrl = "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg"
                }
            },
            new Auction
            {
                Id = Guid.Parse("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"),
                Status = Status.Live,
                ReservePrice = 0,
                Seller = "bob",
                AuctionEnd = DateTime.Parse("2026-06-23T00:08:41Z").ToUniversalTime(),
                Item = new Item
                {
                    Make = "Ford",
                    Model = "Mustang",
                    Color = "Black",
                    Mileage = 65125,
                    Year = 2023,
                    ImageUrl = "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg"
                }
            }
        };
    }
}