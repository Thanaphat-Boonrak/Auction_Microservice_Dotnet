using AuctionService.Data;
using AuctionService.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Status = Grpc.Core.Status;

namespace AuctionService.Services;

public class GrpcAuctionService(AuctionDbContext dbContext) : GrpcAuction.GrpcAuctionBase
{

    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        Console.WriteLine("==> Received Grpc Request for auction");
        var auction = await dbContext.Auctions.FindAsync(Guid.Parse(request.Id));
        
        if(auction == null) throw new RpcException(new Status(StatusCode.NotFound, "Not found"));

        var response = new GrpcAuctionResponse()
        {
            Auction = new GrpcAuctionModel()
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller,
            }
        };
        return response;
    }
}