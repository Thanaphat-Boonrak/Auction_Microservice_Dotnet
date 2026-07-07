using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTest
{
    [Fact]
    public void HasReservedPrice_ReservePriceGtZero_True()
    {
        var auction = new Auction{Id =  Guid.NewGuid(),ReservePrice = 10};


        var result = auction.HasReservePrice();
        Assert.True(result);
    }
    
    [Fact]
    public void HasReservedPrice_ReservePriceGtZero_False()
    {
        var auction = new Auction{Id =  Guid.NewGuid(),ReservePrice = 0};


        var result = auction.HasReservePrice();
        Assert.False(result);
    }
}
