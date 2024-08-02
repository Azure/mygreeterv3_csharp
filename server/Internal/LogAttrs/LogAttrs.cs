using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using Serilog.Core;

namespace LogAttrs;

public static class LogAttributes
{
    private static List<KeyValuePair<string, object>> attrs = new List<KeyValuePair<string, object>>();

    public static void AddAttr(string key, object value)
    {
        attrs.Add(new KeyValuePair<string, object>(key, value));
    }
    
    public static List<KeyValuePair<string, object>> GetAttrs()
    {
        return attrs;
    }
}

// This Serilog enricher will add custom attributes to the log event; it gets called every time a log event occurs, so new attributes will always be included
// https://github.com/serilog/serilog/wiki/Configuration-Basics#enrichers
public class CustomAttributeEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        foreach (var attr in LogAttributes.GetAttrs())
        {
            var property = propertyFactory.CreateProperty(attr.Key, attr.Value);
            logEvent.AddOrUpdateProperty(property);
        }
    }
}