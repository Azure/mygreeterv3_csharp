using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

using ServiceHub.MyGreeterCsharp;

namespace Server;

public static class StartCommand
{
    public static Command Execute()
    {
        var portOption = new Option<int>(
            "--port",
            description: "The address to serve the api on",
            getDefaultValue: () => 50051);
        var jsonLogOption = new Option<bool>(
            "--json-log",
            description: "Enables JSON format for logs (human readable key-value pairs)",
            getDefaultValue: () => false);
        var subscriptionIdOption = new Option<string>(
            "--subscription-id",
            description: "The subscription ID used to access and manage Azure resources",
            getDefaultValue: () => string.Empty);
        var enableAzureSDKCallsOption = new Option<bool>(
            "--enable-azureSDK-calls",
            description: "Toggle to run Azure SDK CRUDL calls if cluster is enabled with workload-id",
            getDefaultValue: () => false);
        var httpPortOption = new Option<int>(
            "--http-port",
            description: "the address to serve the gRPC-Gateway on",
            getDefaultValue: () => 50061);
        var remoteAddrOption = new Option<string>(
            "--remote-addr",
            description: "The demo server's address for this server to connect to",
            getDefaultValue: () => string.Empty);
        var intervalMilliSecOption = new Option<long>(
            "--interval-milli-sec",
            description: "The interval between two requests. Negative numbers mean sending one request.",
            getDefaultValue: () => 0);
        var identityResourceIDOption = new Option<string>(
            "--identity-resource-id",
            description: "The MSI used to authenticate to Azure from E2E env",
            getDefaultValue: () => string.Empty);

        var startCommand = new Command("start", "Start the service")
        {
            portOption,
            jsonLogOption,
            subscriptionIdOption,
            enableAzureSDKCallsOption,
            httpPortOption,
            remoteAddrOption,
            intervalMilliSecOption,
            identityResourceIDOption
        };

        startCommand.Handler = CommandHandler.Create<ServerOptions>(async (options) => await Server.Serve(options));

        return startCommand;
    }
}
