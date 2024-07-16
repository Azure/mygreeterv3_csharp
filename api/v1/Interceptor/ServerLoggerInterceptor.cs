using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog;
using Serilog.Context;
using System.Diagnostics;

namespace MiddlewareListInterceptors;


// Server-side interceptor for logging
public class ServerLoggerInterceptor : Interceptor
{
    private readonly Serilog.ILogger _logger;

    public ServerLoggerInterceptor(Serilog.ILogger logger)
    {
        _logger = logger.ForContext("SourceContext", "ServerLoggerInterceptor");
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {

        var sw = Stopwatch.StartNew();

        LogCall<TRequest, TResponse>(MethodType.Unary, context);

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
        finally
        {
            sw.Stop();
            _logger.Information($"Call duration: {sw.ElapsedMilliseconds} ms");
        }
    }

    private void LogCall<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
        where TRequest : class
        where TResponse : class
    {
        _logger.Warning($"Starting call. Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
        // WriteMetadata(context.RequestHeaders, "caller-user");
        // WriteMetadata(context.RequestHeaders, "caller-machine");
        // WriteMetadata(context.RequestHeaders, "caller-os");
        // WriteMetadata(context.ResponseTrailers, "x-request-id");

        // void WriteMetadata(Metadata headers, string key)
        // {
        //     var headerValue = headers.GetValue(key) ?? "(unknown)";
        //     _logger.Warning($"{key}: {headerValue}");
        // }
    }
}