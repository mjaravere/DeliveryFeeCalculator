using Cronos;
using Microsoft.Extensions.Options;

namespace DeliveryFeeCalculator.Services;

public class WeatherImportBackgroundService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherImportBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public WeatherImportBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<WeatherImportBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cronExpression = _configuration["WeatherImportCron"];
                var parsedExp = CronExpression.Parse(cronExpression);
                var nextRun = parsedExp.GetNextOccurrence(DateTime.UtcNow);

                if (nextRun.HasValue)
                {
                    var delay = nextRun.Value - DateTime.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var weatherImportService = 
                            scope.ServiceProvider.GetRequiredService<WeatherImportService>();
                        await weatherImportService.ImportWeatherData();
                        _logger.LogInformation("Weather data import completed successfully at: {time}", DateTime.UtcNow);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing weather data");
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
} 