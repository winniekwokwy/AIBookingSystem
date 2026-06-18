using AIBookingSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RoomBookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration["dBConnectionString"], o => o.UseNodaTime()));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<DataSeeder>();

var app = builder.Build();

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
        }
    }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
