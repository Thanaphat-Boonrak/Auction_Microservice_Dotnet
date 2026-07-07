using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Cosumers;

public class BidPlaceConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine($"Bid Placed Message Recived");
        await hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}