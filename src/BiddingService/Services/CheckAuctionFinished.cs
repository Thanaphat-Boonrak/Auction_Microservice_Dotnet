using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services;

public class CheckAuctionFinished(ILogger<CheckAuctionFinished> logger,IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => logger.LogInformation("==> Auction is Stopping"));
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuction(stoppingToken);
            
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async  Task CheckAuction(CancellationToken stoppingToken)
    {
        var finishedAuctions = await DB.Default.Find<Auction>().Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => !x.Finished).ExecuteAsync(stoppingToken);
        
        
        if(finishedAuctions.Count == 0) return;
        logger.LogInformation($"==> Auction finished {finishedAuctions.Count} auctions");
        using var scope = services.CreateScope();
        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        foreach (var auction in finishedAuctions)
        {
            auction.Finished = true;
            await DB.Default.SaveAsync(auction, stoppingToken);
            var winningBid = await DB.Default.Find<Bid>().Match(a => a.AuctionId == auction.ID)
                .Match(b => b.BidStatus == BidStatus.Accepted)
                .Sort(x => x.Descending(x => x.Amount)).ExecuteFirstAsync(stoppingToken);
            
            await endpoint.Publish(new AuctionFinished()
            {
                ItemSold = winningBid != null,
                AuctionId = auction.ID,
                Winner = winningBid?.Bidder,
                Amount = winningBid?.Amount,
                Seller = auction.Seller,    
            },stoppingToken);
        }
        
        
    }
}