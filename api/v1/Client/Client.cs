using Grpc.Net.Client;
using Greet;
using Grpc.Core;
using Grpc.Core.Interceptors;

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

using MiddlewareListInterceptors;

namespace Greet.Client
{
    public static class Client
    {
        public static MyGreeter.MyGreeterClient NewClient(string remoteAddr, ILoggerFactory loggerFactory)
        {
            var channel = GrpcChannel.ForAddress($"http://{remoteAddr}", new GrpcChannelOptions
            {
                LoggerFactory = loggerFactory
            });
            var invoker = channel.Intercept(InterceptorFactory.DefaultClientInterceptors(loggerFactory));

            return new MyGreeter.MyGreeterClient(invoker);
        }
    }
}