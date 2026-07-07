using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer(IMapper mapper) : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var item = mapper.Map<Item>(context.Message);

        if (item.Model == "Foo") throw new ArgumentException("Model Cannot be foo");
        
        await DB.Default.SaveAsync(item);
    }
}