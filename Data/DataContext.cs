using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryFeeCalculator.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace DeliveryFeeCalculator.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<WeatherObservation> WeatherObservations{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WeatherObservation>().Property(x => x.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<WeatherObservation>().Property(p => p.Id).HasIdentityOptions(startValue: 4);
            
            modelBuilder.Entity<WeatherObservation>().HasData(
                new WeatherObservation{
                    Id = 1,
                    StationName = "Baas",
                    WMOCode = "uuehd",
                    Temperature = 12,
                    WindSpeed = 3,
                    WeatherPhenomenon = "tuul",
                    Timestamp = DateTime.UtcNow
                },
                new WeatherObservation{
                    Id = 2,
                    StationName = "Ilmajaam",
                    WMOCode = "koht",
                    Temperature = 42,
                    WindSpeed = 7,
                    WeatherPhenomenon = "vihm",
                    Timestamp = DateTime.UtcNow
                }
            );
        }
    }
}