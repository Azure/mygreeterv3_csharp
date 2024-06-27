using Grpc.Core;
using Greet;

namespace Greet.Services;

public partial class GreeterService : MyGreeter.MyGreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly ServerOptions _options;
    
    public GreeterService(ILogger<GreeterService> logger, ServerOptions options)
    {
        _logger = logger;
        _options = options;
    }
}