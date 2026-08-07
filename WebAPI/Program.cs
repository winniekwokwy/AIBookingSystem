using AIBookingSystem.Repositories;
using AIBookingSystem.Services;
using AIBookingSystem.Data;
using AIBookingSystem.Models;
using AIBookingSystem.Helpers;

using System.Net;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RoomBookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration["dBConnectionString"]));
    //options.UseNpgsql(builder.Configuration["dBConnectionString"], o => o.UseNodaTime()));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddTransient<DataSeeder>();

builder.Services.AddMemoryCache();
// Keep the client cache service singleton so its in-memory cache is shared across requests,
// but resolve the repository from a scoped provider each time it needs data.
builder.Services.AddSingleton<IClientCacheService, ClientCacheService>();
builder.Services.AddScoped<IClientCacheRepository, ClientCacheRepository>();
// Register application services with Scoped lifetime (per HTTP request)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
//builder.Services.AddScoped<IUserService, UserService>();
// Declare a Lazy<IClientCacheService> variable to be initialized later 
// This allows deferred resolution of the client cache service after the app is built
Lazy<IClientCacheService>? clientCacheInstance = null;
// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    // Setup token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validate that token issuer matches expected issuer
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // Expected issuer value
        ValidateAudience = false, // Audience is validated manually later in OnTokenValidated
        ValidateIssuerSigningKey = true, // Validate the token's signing key
        ValidateLifetime = true, // Validate token expiration and not-before times
        // Dynamically obtains the signing key based on the client_id claim,
        // fetching the corresponding client’s secret key from cache.
        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
        {
            // Parse the incoming JWT token to extract claims
            var jwtToken = new JwtSecurityToken(token);
            // Extract client_id claim to identify which client signed this token
            var clientId = jwtToken.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
            // If clientId or client cache is not available, return empty keys => fail validation
            if (string.IsNullOrEmpty(clientId) || clientCacheInstance == null)
                return Enumerable.Empty<SecurityKey>();
            // Retrieve the client info synchronously from cache
            var client = clientCacheInstance.Value.GetClientByClientId(clientId).Result;
            if (client == null)
                return Enumerable.Empty<SecurityKey>();
            // Convert the client's stored Base64 secret into a byte array for key
            var keyBytes = Convert.FromBase64String(client.ClientSecret);
            // Create the symmetric security key from byte array for signature validation
            return new[] { new SymmetricSecurityKey(keyBytes) };
        }
    };
    // Additional asynchronous validation after the token is validated,
    // confirming the client exists and audience matches the stored client URL.
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            // Extract client_id claim from the validated token
            var clientId = context.Principal?.FindFirst("client_id")?.Value;
            if (string.IsNullOrEmpty(clientId))
            {
                // Fail if claim is missing
                context.Fail("ClientId claim missing.");
                return;
            }
            if(clientCacheInstance == null)
            {
                context.Fail("Client Cache Instance is null");
                return;
            }
            // Asynchronously get client info from cache or database
            var client = clientCacheInstance.Value.GetClientByClientId(clientId).Result;
            if (client == null)
            {
                // Fail if client not found
                context.Fail("Invalid client.");
                return;
            }
            // Extract audience claim from token and compare to client URL stored in DB/cache
            var audClaim = context.Principal?.FindFirst(JwtRegisteredClaimNames.Aud)?.Value;
            if (audClaim != client.ClientURL)
            {
                // Fail if audience doesn't match
                context.Fail("Invalid audience.");
                return;
            }

            using (var scope = context.HttpContext.RequestServices
                    .CreateScope())
            {
                var tokenService = scope.ServiceProvider
                    .GetRequiredService<ITokenService>();

                var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrWhiteSpace(jti))
                    return;

                var tokenRecord = await tokenService.GetAccessTokenByJtiAsync(jti);

                if (tokenRecord != null && tokenRecord.IsRevoked)
                {
                    context.Fail("Token revoked.");
                    return;
                }
            }
        }
    };
});

var app = builder.Build();
clientCacheInstance = new Lazy<IClientCacheService>(() =>
    app.Services.GetRequiredService<IClientCacheService>());

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        // Data seeding
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            seeder.SeedUsers(7, 3);
            seeder.SeedRooms(20);
            seeder.seedBookings(30, DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow.AddDays(180));
            seeder.seedClients();
        }
    }

if (app.Environment.IsEnvironment("Test"))
{
    app.Use(async (context, next) =>
    {
        context.Connection.RemoteIpAddress =
            IPAddress.Parse("203.0.113.10");

        await next();
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
