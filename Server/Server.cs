using Greet;
using Greet.Services;
using System.Threading.Tasks;

public static class Server
{
    public static async Task Serve(ServerOptions options)
    {
        Console.WriteLine($"Port: {options.Port}");
        Console.WriteLine($"JsonLog: {options.JsonLog}");
        Console.WriteLine($"HTTPPort: {options.HTTPPort}");
        Console.WriteLine($"RemoteAddr: {options.RemoteAddr}");
        Console.WriteLine($"IntervalMilliSec: {options.IntervalMilliSec}");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = new[] { "--urls", $"http://localhost:{options.Port}" }
        });

        // Add services to the container.
        builder.Services.AddGrpc().AddJsonTranscoding();
        builder.Services.AddSingleton(options);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<GreeterService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        await app.RunAsync();
    }
}