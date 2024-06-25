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

        return Task.FromResult(new HelloReply
        {
            Message = "Echo back what you sent me (SayHello): " + request.Name + " " + request.Age.ToString() + " " + request.Email
        });

    }
}
