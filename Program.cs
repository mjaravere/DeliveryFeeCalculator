using DeliveryFeeCalculator.Data;
using DeliveryFeeCalculator.Data.Repos;
using DeliveryFeeCalculator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services
    .AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))
    .AddScoped<WeatherObservationsRepo>()
    .AddHttpClient()
    .AddScoped<WeatherImportService>()
    .AddScoped<FeeCalculator>()
    .AddHostedService<WeatherImportBackgroundService>();
    

var app = builder.Build();

using(var scope = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
using(var context = scope.ServiceProvider.GetService<DataContext>()){
    context?.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
