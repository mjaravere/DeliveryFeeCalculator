using DeliveryFeeCalculator.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace DeliveryFeeCalculator.Data.Repos
{
    public class WeatherObservationsRepo(DataContext context)
    {
        private readonly DataContext context = context;

        public async Task<WeatherObservation> SaveObservationToDb(WeatherObservation weatherObservation){
            context.Add(weatherObservation);
            await context.SaveChangesAsync();
            return weatherObservation;
        }
        public async Task<List<WeatherObservation>> GetAllWeatherObservations() => await context.WeatherObservations.ToListAsync();
        public async Task<WeatherObservation?> GetWeatherObservationById(int id) => await context.WeatherObservations.FindAsync(id);
        public async Task<bool> DeleteWeatherObservation(int id){
            WeatherObservation? weatherObservationInDb = await GetWeatherObservationById(id);
            if (weatherObservationInDb is null){
                return false;
            }
            context.Remove(weatherObservationInDb);
            int changesCount = await context.SaveChangesAsync();

            return changesCount == 1;
        }
        public async Task<WeatherObservation?> GetLatestByStationName(string stationName)
        {
            return await context.WeatherObservations
                .Where(w => w.StationName == stationName)
                .OrderByDescending(w => w.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}