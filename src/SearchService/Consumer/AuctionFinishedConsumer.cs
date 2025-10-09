using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("AuctionFinishedConsumer received a message: " +
                $"{context.Message.AuctionId}, {context.Message.ItemSold}, {context.Message.Winner}, {context.Message.Seller}, {context.Message.Amount}");
            // Since SearchService is read-only, we just log the message for now.
            var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

            if(context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount ?? 0;
                auction.Status = "Finished";
            }
            else if (auction != null)
            {
                auction.Status = auction.SoldAmount > auction.ReservePrice
                    ? "Finished" : "ReserveNotMet";
            }
            await auction.SaveAsync();
        }
    }
}
