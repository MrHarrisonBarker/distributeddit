using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnalyticsService.Consumers;
using AnalyticsService.Contexts;
using AnalyticsService.Models;
using AnalyticsService.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AnalyticsService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Analytics Service", Version = "v1"});
            });
            
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<UserConsumer>();
                cfg.AddConsumer<PostConsumer>();
                cfg.AddConsumer<AuthConsumer>();
                cfg.AddBus(context => Bus.Factory.CreateUsingRabbitMq(rabbitCfg =>
                {
                    rabbitCfg.Host(Configuration["EventBus:HostName"], "/", h =>
                    {
                        h.Username(Configuration["EventBus:UserName"]);
                        h.Password(Configuration["EventBus:Password"]);
                    });
                    rabbitCfg.ConfigureEndpoints(context);
                }));
            });

            services.AddMassTransitHostedService();

            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddTransient<IAnalyticsContext, AnalyticsContext>();
            services.AddScoped<IPostEventService, PostEventService>();
            services.AddScoped<IUserEventService, UserEventService>();
            services.AddScoped<IAuthEventService, AuthEventService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Analytics Service V1"); });
        }
    }
}