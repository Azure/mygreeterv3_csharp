using Grpc.Net.Client;
using Greet;
using Grpc.Core;
using Grpc.Core.Interceptors;

using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

using MiddlewareListInterceptors;

namespace Greet.Client
{
    public static class Client
    {
        public static MyGreeter.MyGreeterClient NewClient(string remoteAddr, ClientInterceptorLogOptions options)
        {

            Log.Logger = options.Logger;

            var channel = GrpcChannel.ForAddress($"http://{remoteAddr}", new GrpcChannelOptions
            {
                LoggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(Log.Logger)
            });
            var invoker = channel.Intercept(InterceptorFactory.DefaultClientInterceptors(options));

            return new MyGreeter.MyGreeterClient(invoker);
        }
    }
}