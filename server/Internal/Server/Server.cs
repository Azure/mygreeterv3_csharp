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
    using System.Runtime.CompilerServices;
    
    using Greet;
    using Greet.Services;
    using Greet.Server;
    using MiddlewareListInterceptors;
    using LogAttrs;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

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

    public static class LoggerExtensions
    {
        public static ILogger WithCallerInformation(this ILogger logger,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "")
        {
            var location = new
            {
                function = callerMemberName,
                file = callerFilePath,
                line = callerLineNumber
            };

            return logger.ForContext("location", location, true);
        }
    }

    public static class Server
    {
        public static async Task Serve(ServerOptions options)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = new[] { "--urls", $"http://0.0.0.0:{options.Port}" }
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(options.Port, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; // Enforce HTTP/2
                });
            });

            // Serilog configuration
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.With(new RemovePropertiesEnricher())
                .Enrich.With<LogAttrs.CustomAttributeEnricher>();

            if (options.JsonLog)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(new ExpressionTemplate(
                    "{ {time: @t, level: if @l = 'Information' then 'INFO' else if @l = 'Error' then 'ERROR' else if @l = 'Warning' then 'WARN' else if @l = 'Debug' then 'DEBUG' else if @l = 'Verbose' then 'VERBOSE' else if @l = 'Fatal' then 'FATAL' else @l, msg: @m, EX: @x, location: @Location, ..@p} }\n"));
            }
            else
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(outputTemplate: "{Timestamp} [{Level}] {Message} {CustomAttributes:lj}{Properties}{NewLine}{Exception}");
            }
            Log.Logger = loggerConfiguration.CreateLogger();

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

            builder.Services.AddGrpcHealthChecks()
                            .AddCheck("GreeterServer", () => HealthCheckResult.Healthy());
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
            app.MapGrpcHealthChecksService();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            await app.RunAsync();
        }
    }
}