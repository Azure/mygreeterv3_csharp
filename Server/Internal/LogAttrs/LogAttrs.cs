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

    public static string FormatAttrs()
    {
        var formattedAttrs = new List<string>();
        foreach (var kvp in attrs)
        {
            formattedAttrs.Add($"{kvp.Key}=\"{kvp.Value}\"");
        }
        return string.Join(", ", formattedAttrs);
    }
    
    public static string GetFormattedAttrs()
    {
        return FormatAttrs();
    }
    public static List<KeyValuePair<string, object>> GetAttrs()
    {
        return attrs;
    }
}

public class CustomAttributeEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var attrs = LogAttributes.FormatAttrs();
        var property = new LogEventProperty("CustomAttributes", new ScalarValue(attrs));
        logEvent.AddOrUpdateProperty(property);
    }
}