using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Client;
//using Default;
//using TestODataCore.Models;
using TestODataCore;

namespace TestClient
{
    class Program
    {
        // TODO: Find out why this is in "DEFAULT" NameSpace
        private static Container _odataContext;

        static void Main(string[] args)
        {
            // set the odata context
            _odataContext = new Container(new Uri("http://achomemachine:18316/odata/"));

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        private static bool MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Get all records.");
            Console.WriteLine("2) Get the latest record.");
            Console.WriteLine("3) Create record with related entities via Batch.");
            Console.WriteLine("4) Exit");
            Console.Write("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    // get all the data in the database
                    GetAllRecords();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "2":
                    // get a single record from the database
                    GetLatestRecord();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "3":
                    // add a record to the database
                    CreateRecordWithRelatedEntities();
                    Console.WriteLine("Hit ENTER to return to menu");
                    Console.ReadLine();
                    return true;
                case "4":
                    return false;
                default:
                    return true;
            }
        }

        static void GetAllRecords()
        {
            try
            {
                var data = _odataContext.WeatherForecasts.Expand(wf => wf.Readings).OrderByDescending(wf => wf.Date).ToList();

                if (data?.Count > 0)
                {
                    Console.WriteLine($"Downloaded {data.Count} records.");

                    foreach (var item in data)
                    {
                        Console.WriteLine($"ID = {item.Id}");
                        Console.WriteLine($"Date = {item.Date}");
                        Console.WriteLine($"Summary = {item.Summary}");
                        Console.WriteLine("");
                    }
                }
                else
                {
                    Console.WriteLine("No records were downloaded.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }
        static void GetLatestRecord()
        {
            try
            {
                var data = _odataContext.WeatherForecasts.Expand(wf => wf.Readings).OrderByDescending(wf => wf.Date).FirstOrDefault();

                if (data != null)
                {
                    Console.WriteLine("Downloaded 1 record.");

                    Console.WriteLine($"ID = {data.Id}");
                    Console.WriteLine($"Date = {data.Date}");
                    Console.WriteLine($"Summary = {data.Summary}");
					Console.WriteLine($"Readings Count = {data.Readings.Count}");
					for (var i = 0; i < data.Readings.Count; i++)
					{
						var reading = data.Readings[i];
                        Console.WriteLine($"Reading #{i + 1} = {reading.Temperature} {reading.TemperatureSystem}");
                    }

					Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("No record was downloaded.");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }
        static void CreateRecordWithRelatedEntities()
        {
            try
            {
                var random = new Random();
                var weatherForecast = new WeatherForecast()
                {
                    Id = Guid.NewGuid(),
                    Date = DateTimeOffset.Now,
                    Summary = "Human entered"
                };
                var reading1 = new WeatherReading()
                {
                    Id = Guid.NewGuid(),
                    ForecastId = weatherForecast.Id,
				    Temperature = random.Next(-20, 55),
                    TemperatureSystem = "C"
                };

				weatherForecast.Readings.Add(reading1);

				_odataContext.AddToWeatherForecasts(weatherForecast);
                _odataContext.AddRelatedObject(weatherForecast, nameof(WeatherForecast.Readings), reading1);

                _odataContext.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

                WeatherForecast data = _odataContext.WeatherForecasts.Where(w => w.Id == weatherForecast.Id).FirstOrDefault();

                if (data != null)
                {
                    Console.WriteLine("Added 1 record.");

                    Console.WriteLine($"ID = {data.Id}");
                    Console.WriteLine($"Date = {data.Date}");
                    Console.WriteLine($"Summary = {data.Summary}");
                    Console.WriteLine($"Readings Count = {data.Readings.Count}");
                    for (var i = 0; i < data.Readings.Count; i++)
                    {
                        var reading = data.Readings[i];
                        Console.WriteLine($"Reading #{i + 1} = {reading.Temperature} {reading.TemperatureSystem}");
                    }
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("No record was added.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
