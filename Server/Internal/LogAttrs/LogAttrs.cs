using Serilog;
using Serilog.Events;
using System.Collections.Generic;

namespace LogAttrs;

public static class LogAttributes
{
    private static List<LogEventProperty> attrs = new List<LogEventProperty>();

    public static void AddAttr(string key, object value)
    {
        var logEventProperty = new LogEventProperty(key, new ScalarValue(value));
        attrs.Add(logEventProperty);
    }

    public static List<LogEventProperty> GetAttrs()
    {
        return attrs;
    }
}
