using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;
using Serilog.Context;
using Serilog.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MiddlewareListInterceptors;

public class ClientInterceptorLogOptions
{
    public ILogger Logger { get; set; }
    public TextWriter APIOutput { get; set; }
    public List<KeyValuePair<string, object>> Attributes { get; set; }
}

public class ServerInterceptorLogOptions
{
    public ILogger Logger { get; set; }
    public TextWriter APIOutput { get; set; }
    public TextWriter CtxOutput { get; set; }
    public List<KeyValuePair<string, object>> APIAttributes { get; set; }
    public List<KeyValuePair<string, object>> CtxAttributes { get; set; }
}

public static class InterceptorLogOptionsFactory
{
    public static ClientInterceptorLogOptions GetClientInterceptorLogOptions(ILogger logger, List<KeyValuePair<string, object>> attrs)
    {
        return new ClientInterceptorLogOptions
        {
            Logger = logger,
            APIOutput = Console.Out,
            Attributes = attrs
        };
    }

    public static ServerInterceptorLogOptions GetServerInterceptorLogOptions(ILogger logger, List<KeyValuePair<string, object>> attrs)
    {
        return new ServerInterceptorLogOptions
        {
            Logger = logger,
            APIOutput = Console.Out,
            CtxOutput = Console.Out,
            APIAttributes = attrs,
            CtxAttributes = attrs
        };
    }
}


public class InterceptorFactory
{

    public static Interceptor[] DefaultClientInterceptors(ClientInterceptorLogOptions options)
    {

        var logger = options.Logger;

        var interceptors = new Interceptor[]
        {
            new RetryInterceptor(),
            new MdForwardInterceptor(),
            new ClientLoggerInterceptor(logger)
        };

        return interceptors;
    }

    public static Interceptor[] DefaultServerInterceptors(ServerInterceptorLogOptions options)
    {

        var logger = options.Logger;
        
        var interceptors = new Interceptor[]
        {
            new ValidationInterceptor(logger),
            new RequestIdInterceptor(logger),
            new CtxLoggerInterceptor(logger),
            new ServerLoggerInterceptor(logger)
        };

        return interceptors;
    }
}
