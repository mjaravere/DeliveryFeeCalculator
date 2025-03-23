using System.Reflection;
using DeliveryFeeCalculator.Data;
using DeliveryFeeCalculator.Data.Repos;
using DeliveryFeeCalculator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather API", Version = "v1" });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
