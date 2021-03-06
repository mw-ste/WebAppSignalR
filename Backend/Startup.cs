using Microsoft.Azure.SignalR;
using Microsoft.Extensions.Logging;

namespace Backend
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSignalR()
                .AddAzureSignalR(options => 
                    options.GracefulShutdown.Mode = GracefulShutdownMode.MigrateClients); // just to check ;-)

            services.AddControllers();
            services.AddSwaggerGen();

            services.AddSingleton(provider => new EvilSurveillanceLogger(provider.GetService<ILogger<EvilSurveillanceLogger>>()));

            services.AddHostedService<EvilSurveillanceBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppSignalR API");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CommunicationSignalRHub>("/communicationsignalrhub");
                endpoints.MapControllers();
            });
        }
    }
}
