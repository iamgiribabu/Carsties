using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            Console.WriteLine($"AuctionDeletedConsumer: Auction deleted with ID {context.Message.Id}");
            var result = await DB.DeleteAsync<Item>(context.Message.Id);   
            if(!result.IsAcknowledged) 
            {
                throw new MessageException(typeof(AuctionDeleted), $"Failed to delete item with ID {context.Message.Id} in MongoDB");
            }
        }
    }
}
