using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
        {

        }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Item> Items { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Auction>()
            //    .HasOne(a => a.Item)
            //    .WithOne(i => i.Auction)
            //    .HasForeignKey<Item>(i => i.AuctionId);
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    // Set table name to lowercase
            //    entity.SetTableName(entity.GetTableName()!.ToLower());

            //    // Set column names to lowercase
            //    foreach (var property in entity.GetProperties())
            //    {
            //        property.SetColumnName(property.Name.ToLower());
            //    }
            //}
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    // Set quoted PascalCase table name
            //    var tableName = entity.GetTableName();
            //    entity.SetTableName(tableName);

            //    // Set quoted PascalCase column names
            //    foreach (var property in entity.GetProperties())
            //    {
            //        var columnName = property.Name;
            //        property.SetColumnName(columnName);
            //    }
            //}


            //base.OnModelCreating(modelBuilder);
        }
    }
    
}
