using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"]; // IdentityServer URL
        options.RequireHttpsMetadata = false; // For development only
        options.TokenValidationParameters.ValidateAudience = false; // Disable audience validation
        options.TokenValidationParameters.NameClaimType = "username"; // Set the name claim type

        // Add these debug handlers
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("Challenge triggered: " + context.Error);
                return Task.CompletedTask;
            }
        };


    });

builder.Services.AddLogging(logging =>
{
    logging.AddConsole()
           .AddDebug()
           .SetMinimumLevel(LogLevel.Debug);
});
var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.Run();
