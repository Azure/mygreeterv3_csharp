using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog;
using Serilog.Context;

namespace MiddlewareListInterceptors;

public class CtxLoggerInterceptor : Interceptor
{
    private readonly Serilog.ILogger _logger;
    private const string methodLogKey = "method";
    private const string requestContentLogKey = "request";

    public CtxLoggerInterceptor(Serilog.ILogger logger)
    {
        _logger = logger.ForContext("source", "CtxLog");
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {

        LogContext.PushProperty(methodLogKey, context.Method);
        LogContext.PushProperty(requestContentLogKey, request, destructureObjects: true);
        _logger.Information("within the ctx logger!!");

        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            // Note: The gRPC framework also logs exceptions thrown by handlers to .NET Core logging.
            _logger.Error(ex, $"Error thrown by {context.Method}.");

            throw;
        }
    }
}
