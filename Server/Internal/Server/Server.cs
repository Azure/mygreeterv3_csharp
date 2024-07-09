namespace Greet.Server {

    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Serilog.Extensions.Logging;
    using Serilog.Formatting.Compact;
    using System.Globalization;

    using Greet;
    using Greet.Services;
    using Greet.Server;
    using MiddlewareListInterceptors;
    using LogAttrs;

    public static class Server
    {
        public static async Task Serve(ServerOptions options)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = new[] { "--urls", $"http://localhost:{options.Port}" }
            });

            // Serilog configuration
            var loggerConfiguration = new LoggerConfiguration();

            if (options.JsonLog)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
            }
            else
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console();
            }
            Log.Logger = loggerConfiguration.CreateLogger();
            
            builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
            builder.Services.AddSingleton(options);

            ServerInterceptorLogOptions interceptorOptions = InterceptorLogOptionsFactory.GetServerInterceptorLogOptions(Log.Logger, LogAttributes.GetAttrs());

            // Add services to the container.
            builder.Services.AddGrpc(options =>
            {
                // Add your custom server interceptors
                var serverInterceptors = InterceptorFactory.DefaultServerInterceptors(interceptorOptions);
                foreach (var interceptor in serverInterceptors)
                {
                    options.Interceptors.Add(interceptor.GetType());
                }
            }).AddJsonTranscoding();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            await app.RunAsync();
        }
    }
}