using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestODataCore.DbContexts;
using TestODataCore.Models;

namespace TestODataCore.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class WeatherReadingsController : ODataController
    {
        private ApiContext _dbContext;

        public WeatherReadingsController(ApiContext context)
        {
            _dbContext = context;
        }

        [EnableQuery]
        public IQueryable<WeatherReading> Get()
        {
            return _dbContext.WeatherReadings;
        }

        [EnableQuery]
        public SingleResult<WeatherReading> Get([FromODataUri] Guid key)
        {
            IQueryable<WeatherReading> result = _dbContext.WeatherReadings.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IActionResult> Post(WeatherReading WeatherReading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.WeatherReadings.Add(WeatherReading);

            await _dbContext.SaveChangesAsync();

            return Created(WeatherReading);
        }
        public async Task<IActionResult> Patch([FromODataUri] Guid key, Delta<WeatherReading> weatherReading)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbContext.WeatherReadings.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            weatherReading.Patch(entity);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherReadingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }
        public async Task<IActionResult> Put([FromODataUri] Guid key, WeatherReading update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(update).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherReadingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(update);
        }
        public async Task<IActionResult> Delete([FromODataUri] Guid key)
        {
            var WeatherReading = await _dbContext.WeatherReadings.FindAsync(key);
            if (WeatherReading == null)
            {
                return NotFound();
            }

            _dbContext.WeatherReadings.Remove(WeatherReading);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool WeatherReadingExists(Guid key)
        {
            return _dbContext.WeatherReadings.Any(p => p.Id == key);
        }
    }
}
