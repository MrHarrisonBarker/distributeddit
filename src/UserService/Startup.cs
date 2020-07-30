using System;
using System.Reflection;
using AutoMapper;
using BuildingBlocks.Models;
using Infrastructure.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UserService.Consumers;
using UserService.Contexts;

namespace UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public  IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"Db connection string used {Configuration["ConnectionString"]}");
            services.AddDbContext<UserContext>(options =>
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
            
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            
            services.AddAutoMapper(typeof(Startup));
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service", Version = "v1" });
            });

            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<AuthenticateUserConsumer>();
                cfg.AddConsumer<AddPostConsumer>();
                cfg.AddConsumer<DestroyPostConsumer>();
                cfg.AddBus(context => Bus.Factory.CreateUsingRabbitMq(rabbitCfg =>
                {
                    rabbitCfg.Host(Configuration["EventBus:HostName"], "/", h =>
                    {
                        h.Username(Configuration["EventBus:UserName"]);
                        h.Password(Configuration["EventBus:Password"]);
                    });
                    rabbitCfg.ConfigureEndpoints(context);
                }));
                cfg.AddRequestClient<DestroyUserPostsRequest>();
                cfg.AddRequestClient<GetUserPostsRequest>();
            });

            services.AddMassTransitHostedService();

            services.AddScoped<IUserService, UserService>();
        }
        
        IBusControl ConfigureBus(IServiceProvider provider)
        {
            // var options = provider.GetRequiredService<IOptions<AppConfig>>().Value;

            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(Configuration["EventBus:HostName"], "/", h =>
                {
                    h.Username(Configuration["EventBus:UserName"]);
                    h.Password(Configuration["EventBus:Password"]);
                });

                // cfg.ReceiveEndpoint();

                // cfg.UseInMemoryScheduler();
                //
                // var serviceInstanceOptions = new ServiceInstanceOptions()
                //     .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);
                //
                // cfg.ConfigureServiceEndpoints(provider, serviceInstanceOptions);
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Service V1");
            });
        }
        
        private void ConfigureConsul(IServiceCollection services)
        {
            var serviceConfig = Configuration.GetServiceConfig();

            services.RegisterConsulServices(serviceConfig);
        }
    }

    
}
