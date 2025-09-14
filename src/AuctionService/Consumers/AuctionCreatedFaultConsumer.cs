using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine($"AuctionCreatedFaultConsumer: Auction creation failed with ID {context.Message.Message.Id}");
            var exception = context.Message.Exceptions.FirstOrDefault();
            Console.WriteLine($"Fault: {context.Message.Exceptions.FirstOrDefault()?.Message}");
            // Handle the fault, e.g., log it or take corrective action

            if(exception.ExceptionType == "System.ArgumentException")
            {
                context.Message.Message.Model = "Foobar";
                await context.Publish(context.Message.Message);
                
            }
            else
            {
                Console.WriteLine("An unexpected error occurred, handling it generically.");
                // Handle other types of exceptions
            }
        }
    }
}
