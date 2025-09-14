using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using AuctionService.Consumers; // Add this


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit(x =>
{
    // Add Entity Framework Outbox
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        
        // Configure how often the delivery/cleanup process runs
        o.QueryDelay = TimeSpan.FromSeconds(10);

        // Configure the outbox to use SQL Server
        o.UseSqlServer(); // Specify we're using SQL Server


        // Use optimistic concurrency for SQL Server
        o.UseBusOutbox();
    });
    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.  

app.UseAuthorization();

app.MapControllers();

try
{
    // Initialize the database with seed data  
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
    throw;
}
app.Run();
