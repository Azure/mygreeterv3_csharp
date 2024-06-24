using Grpc.Core;
using Greet;

namespace Greet.Services;

public class GreeterService : MyGreeter.MyGreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly ServerOptions _options;
    
    public GreeterService(ILogger<GreeterService> logger, ServerOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"ServerOptions - Port: {_options.Port}");

        Console.WriteLine(request.Name);

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name + "on port " + _options.Port + "!"
        });
    }
}
