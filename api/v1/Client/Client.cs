using Grpc.Net.Client;
using ServiceHub.MyGreeter;
using Grpc.Core;
using Grpc.Core.Interceptors;

using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

using AKSMiddleware;

namespace Client;

public static class ClientFactory
{
    public static MyGreeter.MyGreeterClient NewClient(string remoteAddr, ILogger logger)
    {
        var channel = GrpcChannel.ForAddress($"http://{remoteAddr}", new GrpcChannelOptions
        {
            LoggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(logger)
        });
        var invoker = channel.Intercept(InterceptorFactory.DefaultClientInterceptors(logger));

        return new MyGreeter.MyGreeterClient(invoker);
    }
}