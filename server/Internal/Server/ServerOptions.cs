using Greet.Server;

namespace Greet
{
    public class ServerOptions
    {
        public int Port { get; set; }
        public bool JsonLog { get; set; }
        public int HTTPPort { get; set; }
        public string? RemoteAddr { get; set; }
        public long IntervalMilliSec { get; set; }
        public string? SubscriptionID { get; set; }
        public bool EnableAzureSDKCalls { get; set; }
        public string? IdentityResourceID { get; set; }
    }
}
