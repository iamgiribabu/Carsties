using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;
        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine($"AuctionUpdatedConsumer: Auction updated with ID {context.Message.Id}");
            // Here you would typically update your search index with the new auction details
            // For demonstration, we just print the details to the console
            var item = _mapper.Map<Item>(context.Message);

            var result = await DB.Update<Item>()
                .Match(a => a.ID == context.Message.Id)
                .ModifyOnly(x => new { x.Make, x.Model, x.Year, x.Color, x.Mileage }, item)
                .ExecuteAsync();

            if (!result.IsAcknowledged)
            {
                throw new MessageException(typeof(AuctionUpdated), $"Failed to update item with ID {context.Message.Id} in MongoDB");
            }

            Console.WriteLine($"Updated Auction Details: Make={context.Message.Make}, Model={context.Message.Model}, Year={context.Message.Year}, Color={context.Message.Color}, Mileage={context.Message.Mileage}");
            
        }
    }
}
