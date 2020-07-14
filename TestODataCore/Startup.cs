using System;
using System.Linq;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using TestODataCore.DbContexts;
using TestODataCore.Models;

namespace TestODataCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApiContext>(options => options.UseInMemoryDatabase(databaseName: "ODataCoreTestDb"));

            // Newer endpoint routing.
            //services.AddControllers();

            // Older non-endpoint routing.
            var mvcBuilder = services.AddControllers();
            mvcBuilder.AddControllersAsServices();
            mvcBuilder.AddMvcOptions(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddNewtonsoftJson();

            services.AddOData();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseODataBatching();

            // Newer endpoint routing.
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    endpoints.Select().Filter().OrderBy().Count().MaxTop(10);
            //    endpoints.MapODataRoute("odata", "odata", GetEdmModel());
            //});

            // Older non-endpoint routing.
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
                routeBuilder.MapODataServiceRoute(
                    routeName: "odata",
                    routePrefix: "odata",
                    model: GetEdmModel(),
                    pathHandler: new DefaultODataPathHandler(),
                    routingConventions: ODataRoutingConventions.CreateDefault(),
                    batchHandler: new DefaultODataBatchHandler());
            });
        }

        private IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();

            // TODO: Find out how to get this to set the namespace for models different than the namespace for the controllers
            odataBuilder.Namespace = "TestODataCore";
            odataBuilder.EntitySet<WeatherForecast>("WeatherForecasts");
			odataBuilder.EntitySet<WeatherReading>("WeatherReadings");

			return odataBuilder.GetEdmModel();
        }
    }
}