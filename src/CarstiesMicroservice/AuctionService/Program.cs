using AuctionService.Data;
using Microsoft.EntityFrameworkCore; // Add this using directive to resolve 'UseSqlServer'  

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
