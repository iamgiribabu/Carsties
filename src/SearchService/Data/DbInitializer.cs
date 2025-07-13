using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDB", MongoClientSettings
                    .FromConnectionString(app.Configuration
                    .GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(i => i.Make, KeyType.Text)
                .Key(i => i.Model, KeyType.Text)
                .Key(i => i.Year, KeyType.Text)
                .Key(i => i.Color, KeyType.Text)
                .Key(i => i.Mileage, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();
            using var scope = app.Services.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
            var items = await httpClient.GetItemForSearchDb();

            Console.WriteLine($"Items fetched from Auction Service: {items.Count}");
            if (items.Count > 0)
            {
                Console.WriteLine("Initializing Search DB with items from Auction Service...");
                await DB.SaveAsync(items);
            }
           

        }
    }
}
