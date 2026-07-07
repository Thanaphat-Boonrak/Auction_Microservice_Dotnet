using AutoMapper;
using AutoMapper.QueryableExtensions;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiddingService.Controllers;

public class BidsController(IMapper mapper,IPublishEndpoint publishEndpoint,GrpcAuctionClient grpcAuctionClient) : BaseController
{
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDto>> PlaceBid(string auctionId,int amount)
    {
        var auction = await database.Find<Auction>().OneAsync(auctionId);

        if (auction == null)
        {
            auction = grpcAuctionClient.GetAuction(auctionId);
            if (auction == null) return BadRequest("Cannot bid this auction");
                
        }

        if (auction.Seller == User.Identity!.Name)
        {
            return BadRequest("You cannot place a Bid your own auction");
        }

        var bid = new Bid()
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity.Name!
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.Finished;
        }
        else
        {
            
            var highBid = await database.Find<Bid>().Match(a => a.AuctionId == auctionId)
                .Sort(b => b.Descending(x => x.Amount)).ExecuteFirstAsync();
            if (highBid != null && amount > highBid.Amount || highBid == null)
            {
                bid.BidStatus = amount > auction.ReservedPrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;
            }

            if (highBid != null && bid.Amount <= highBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }
       

        await  database.SaveAsync(bid);
        
        await publishEndpoint.Publish(mapper.Map<BidPlaced>(bid));
        
        return Ok(mapper.Map<BidDto>(bid));
    }


    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<BidDto>>> GetBid(string auctionId)
    {
        var bids = await database.Find<Bid>().Match(a => a.AuctionId == auctionId).Sort(b => b.Ascending(a => a.BidTime))
            .ExecuteAsync();
        return Ok(bids.AsQueryable().ProjectTo<BidDto>(mapper.ConfigurationProvider).ToList());
    }
    
}