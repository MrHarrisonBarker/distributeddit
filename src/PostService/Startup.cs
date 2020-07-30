using System;
using System.Reflection;
using BuildingBlocks.Models;
using Infrastructure.ServiceDiscovery;
using MassTransit;
using MassTransit.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PostService.Consumers;
using PostService.Contexts;

namespace PostService
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
            services.AddDbContext<PostContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(
                            typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                        //Configuring Connection Resiliency:
                        sqlOptions.
                            EnableRetryOnFailure(maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);

                    });

                // Changing default behavior when client evaluation occurs to throw.
                // Default in EFCore would be to log warning when client evaluation is done.
                options.ConfigureWarnings(warnings => warnings.Throw(
                    RelationalEventId.QueryClientEvaluationWarning));
            });
            
            ConfigureConsul(services);

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Post Service", Version = "v1"});
            });

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<DestroyUserPostsConsumer>();
                cfg.AddConsumer<GetUserPostsConsumer>();
                cfg.AddBus(context => Bus.Factory.CreateUsingRabbitMq(rabbitCfg =>
                {
                    rabbitCfg.Host(Configuration["EventBus:HostName"], "/", h =>
                    {
                        h.Username(Configuration["EventBus:UserName"]);
                        h.Password(Configuration["EventBus:Password"]);
                    });
                    rabbitCfg.ConfigureEndpoints(context);
                }));
                cfg.AddRequestClient<Post>();
                cfg.AddRequestClient<DestroyPostRequest>();
            });

            services.AddMassTransitHostedService();


            services.AddScoped<IPostService, PostService>();
        }

        IBusControl ConfigureBus(IServiceProvider provider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(Configuration["EventBus:HostName"], "/", h =>
                {
                    h.Username(Configuration["EventBus:UserName"]);
                    h.Password(Configuration["EventBus:Password"]);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Console.WriteLine("Dev is enabled");
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Post Service V1"); });
        }

        private void ConfigureConsul(IServiceCollection services)
        {
            var serviceConfig = Configuration.GetServiceConfig();
        
            services.RegisterConsulServices(serviceConfig);
        }
    }
}