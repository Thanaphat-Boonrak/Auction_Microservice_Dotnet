using System.Text.Json;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Consumers;

public class AuctionUpdateConsumer(IMapper mapper, ILogger<AuctionUpdateConsumer> logger) : IConsumer<UpdateAuction>
{
    public async Task Consume(ConsumeContext<UpdateAuction> context)
    {
        var message = context.Message;
        var result = await DB.Default.Update<Item>()
            .Match(i => i.ID == message.Id)
            .Modify(b => b.Set(x => x.Make, message.Make)
                .Set(x => x.Model, message.Model)
                .Set(x => x.Year, message.Year)
                .Set(x => x.Color, message.Color)
                .Set(x => x.Mileage, message.Mileage))
            .ExecuteAsync();
    }

}