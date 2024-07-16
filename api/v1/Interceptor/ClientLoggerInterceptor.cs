using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog;
using Serilog.Context;

namespace MiddlewareListInterceptors;


// Client-side interceptor for logging
public class ClientLoggerInterceptor : Interceptor
{
    private readonly Serilog.ILogger _logger;

    public ClientLoggerInterceptor(Serilog.ILogger logger)
    {
        _logger = logger.ForContext("source", "clientLoggerInterceptor");
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        LogCall<TRequest, TResponse>(context.Method);

        // Add caller metadata to headers
        AddCallerMetadata(ref context);

        LogContext.PushProperty("test key", "test value");

        // Log the start of the call
        _logger.Information("Starting call. Type/Method: {Type} / {Method}",
            context.Method.Type, context.Method.Name);

        // Continue the call chain
        var response = continuation(request, context);

        // Handle response asynchronously
        Task.Run(() => HandleResponse(response));

        return response;
    }

    private async Task<TResponse> HandleResponse<TResponse>(AsyncUnaryCall<TResponse> call)
    {
        try
        {
            var response = await call.ResponseAsync;
            _logger.Information($"Response received: {response}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Call error: {ex.Message}");
            throw;
        }
    }

    private void LogCall<TRequest, TResponse>(Method<TRequest, TResponse> method)
        where TRequest : class
        where TResponse : class
    {
        _logger.Information($"Starting call. Name: {method.Name}. Type: {method.Type}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
    }

    private void AddCallerMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
        where TRequest : class
        where TResponse : class
    {
        var headers = context.Options.Headers;

        // Create new headers if they don't exist
        if (headers == null)
        {
            headers = new Metadata();
            var options = context.Options.WithHeaders(headers);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);
        }

        // Add caller metadata to headers
        headers.Add("caller-user", Environment.UserName);
        headers.Add("caller-machine", Environment.MachineName);
        headers.Add("caller-os", Environment.OSVersion.ToString());
    }

}