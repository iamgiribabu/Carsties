using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _context;
        public BidPlacedConsumer(AuctionDbContext dbcontext)
        {
            _context = dbcontext;
        }
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("BidPlacedConsumer received a message: " +
                $"{context.Message.Id}, {context.Message.AuctionId}, {context.Message.Bidder}, {context.Message.Amount}, {context.Message.BidTime}, {context.Message.BidStatus}");
            var auction = await _context.Auctions.FindAsync(context.Message.AuctionId);
            if(auction.CurrentHighBid == null || context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
               
                await _context.SaveChangesAsync();
            }
        }
    }
}
