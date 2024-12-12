using Microsoft.Extensions.Logging.Console;

namespace ContinentDemo.WebApi
{
    using Caching;
    using Interfaces;
    using FluentValidation;
    using Logic;
    using Middleware;
    using Services;
    using Queries;
    using Microsoft.OpenApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            //logs
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                })
            );

            ILogger<NetworkRequestHandler> networkHandlerLogger = loggerFactory.CreateLogger<NetworkRequestHandler>();
            ILogger<ProblemDetails> problemDetailsLogger = loggerFactory.CreateLogger<ProblemDetails>();
            //ILogger<DistanceService> distServiceLogger = loggerFactory.CreateLogger<DistanceService>();

            builder.Services.AddTransient<IDistanceService, DistanceService>();
            builder.Services.AddTransient<ILocationLogic, LocationLogic>();
            builder.Services.AddTransient<INetworkRequestHandler>(_ =>
                new NetworkRequestHandler(ConfigAppSettings.NetWorkRequestHost, ConfigAppSettings.NetWorkRequestUri, networkHandlerLogger));

            if (ConfigAppSettings.UseDistributedCache) { 
                //distr. cache
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = ConfigAppSettings.RedisCacheConfigurationString;
                    options.InstanceName = ConfigAppSettings.RedisCacheInstanceName;
                });
                builder.Services.AddTransient<ICacheStorage, DistributedCacheStorage>();
            }
            else {
                //local cache
                builder.Services.AddSingleton<ICacheStorage, LocalCacheStorage>();
            }

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ContinentDemo API",
                    Version = "v1",
                    Description = "...",
                    Contact = new OpenApiContact
                    {
                        Name = "Dmitry",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/Dmitry-github/"),
                    },
                });
            });

            //FluentValidation
            builder.Services.AddScoped<IValidator<DistanceQuery>, TransactionQueryValidator>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContinentDemo API V1");
                    // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                    c.RoutePrefix = "swagger"; // * to launchSettings.json launchUrl
                });
            }

            app.ConfigureExceptionHandler(problemDetailsLogger);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            
            //app.MapControllers();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}