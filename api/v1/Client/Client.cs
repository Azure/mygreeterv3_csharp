using Grpc.Net.Client;
using ServiceHub.MyGreeterCsharp;
using Grpc.Core;
using Grpc.Core.Interceptors;

using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

using AKSMiddleware;

namespace Client;

public static class ClientFactory
{
    public static MyGreeterCsharp.MyGreeterCsharpClient NewClient(string remoteAddr, ILogger logger)
    {
        var channel = GrpcChannel.ForAddress($"http://{remoteAddr}", new GrpcChannelOptions
        {
            LoggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(logger)
        });
        var invoker = channel.Intercept(InterceptorFactory.DefaultClientInterceptors(logger));

        return new MyGreeterCsharp.MyGreeterCsharpClient(invoker);
    }
}