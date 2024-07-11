using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Serilog.Context;

namespace MiddlewareListInterceptors;

public class RequestIdInterceptor : Interceptor
{
    public const string RequestIDMetadataKey = "x-request-id";
    public const string RequestIDLogKey = "request-id";

    private readonly Serilog.ILogger _logger;

    public RequestIdInterceptor(Serilog.ILogger logger)
    {
        _logger = logger.ForContext("source", "RequestIdInterceptor");
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        context = GenerateRequestID(context);

        LogContext.PushProperty("requestid", GetRequestID(context));
        _logger.Information("inside the requestid interceptor");
        
        return await continuation(request, context);

    }

    private static ServerCallContext GenerateRequestID(ServerCallContext context)
    {
        if (context.RequestHeaders.GetValue(RequestIDMetadataKey) is null)
        {
            string shortId = ShortID();
            context.ResponseTrailers.Add(RequestIDMetadataKey, shortId);
        }
        return context;
    }

    private static string ShortID()
    {
        byte[] buffer = new byte[6];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(buffer);
        }
        return Base64UrlEncode(buffer);
    }

    private static string Base64UrlEncode(byte[] buffer)
    {
        return Convert.ToBase64String(buffer)
                        .TrimEnd('=')
                        .Replace('+', '-')
                        .Replace('/', '_');
    }

    public static string GetRequestID(ServerCallContext context)
    {
        return context.RequestHeaders.GetValue(RequestIDMetadataKey) 
           ?? context.ResponseTrailers.GetValue(RequestIDMetadataKey) 
           ?? string.Empty;
    }
}
