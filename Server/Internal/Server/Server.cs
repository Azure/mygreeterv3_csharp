namespace Greet.Server {

    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using Serilog.Context;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Extensions.Logging;
    using Serilog.Formatting;
    using Serilog.Formatting.Compact;
    using Serilog.Templates;

    
    using Greet;
    using Greet.Services;
    using Greet.Server;
    using MiddlewareListInterceptors;
    using LogAttrs;

    class RemovePropertiesEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            le.RemovePropertyIfPresent("SourceContext");
            le.RemovePropertyIfPresent("RequestId");
            le.RemovePropertyIfPresent("RequestPath");
            le.RemovePropertyIfPresent("ConnectionId");
        }
    }

    public static class Server
    {
        public static async Task Serve(ServerOptions options)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = new[] { "--urls", $"http://localhost:{options.Port}" }
            });

            // Serilog configuration
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.With(new RemovePropertiesEnricher())
                .Enrich.With<LogAttrs.CustomAttributeEnricher>();

            if (options.JsonLog)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(new ExpressionTemplate("{ {time: @t, level: @l, msg: @m, EX: @x, ..@p} }\n"));
            }
            else
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(outputTemplate: "{Timestamp} [{Level}] {Message} {CustomAttributes:lj}{Properties}{NewLine}{Exception}");
            }
            Log.Logger = loggerConfiguration.CreateLogger();

            Log.Logger = Log.Logger.ForContext("Permanent key", "permanent value");

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);
            
            builder.Services.AddScoped<ILogger>(provider =>
            {
                var logger = Log.Logger;
                return logger;
            });
            builder.Services.AddSingleton(options);

            // Add services to the container.
            builder.Services.AddGrpc(options =>
            {
                // Add your custom server interceptors
                var serverInterceptors = InterceptorFactory.DefaultServerInterceptors(Log.Logger);
                foreach (var interceptor in serverInterceptors)
                {
                    options.Interceptors.Add(interceptor.GetType());
                }
            }).AddJsonTranscoding();

            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });

                var filePath = Path.Combine(AppContext.BaseDirectory, "GrpcGreeter.xml"); // Adjust as per your project setup
                c.IncludeXmlComments(filePath);
                c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
            });

            var app = builder.Build();

            // app.UseSwagger();
            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwaggerUI(c =>
            //     {
            //         c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //     });
            // }

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            await app.RunAsync();
        }
    }
}