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

    }
}