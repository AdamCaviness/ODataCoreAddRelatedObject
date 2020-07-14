using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestODataCore.Models;

namespace TestODataCore.DbContexts
{
    public class DbContextSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var randomTime = new Random();
			var randomTemp = new Random();
			using (var context = new ApiContext(serviceProvider.GetRequiredService<DbContextOptions<ApiContext>>()))
            {
				for (int i = 1; i < 6; i++)
                {
                    var forecast = new WeatherForecast()
                    {
                        Id = Guid.NewGuid(),
                        Date = DateTime.Now.AddSeconds(randomTime.Next(1, 5) * -1),
                        Summary = "Record " + i.ToString()
                    };
                    var reading = new WeatherReading()
                    {
                        Id = Guid.NewGuid(),
                        Temperature = randomTemp.Next(-20, 55),
                        TemperatureSystem = "C"
                    };

                    forecast.Readings.Add(reading);

                    context.WeatherForecasts.Add(forecast);
                    context.WeatherReadings.Add(reading);
                }

                context.SaveChanges();
            }
        }
    }
}