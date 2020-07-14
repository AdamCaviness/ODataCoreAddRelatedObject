using System;
using System.Collections.Generic;

namespace TestODataCore.Models
{
    public class WeatherForecast
    {
		public WeatherForecast()
		{
            Readings = new HashSet<WeatherReading>();
		}

        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Summary { get; set; }

        public virtual ICollection<WeatherReading> Readings { get; set; }
    }
}
