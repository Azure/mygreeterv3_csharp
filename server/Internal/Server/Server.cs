namespace Greet.Server {

    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Serilog;
    using Serilog.Context;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Templates;
    using Grpc.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    
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

            // using Serilog ExpressionTemplate
            // https://github.com/serilog/serilog-expressions?tab=readme-ov-file#formatting-with-expressiontemplate
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

                var filePath = Path.Combine(AppContext.BaseDirectory, "GrpcGreeter.xml");
                c.IncludeXmlComments(filePath);
                c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGrpcHealthChecksService();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            await app.RunAsync();
        }
    }
}