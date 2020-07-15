using System;

namespace TestODataCore.Models
{
	public class WeatherReading
	{
		public Guid Id { get; set; }

		public Guid ForecastId { get; set; }

		public int Temperature { get; set; }
		public string TemperatureSystem { get; set; }

		public virtual WeatherForecast Forecast { get; set; }
	}
}