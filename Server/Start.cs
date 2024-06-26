using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

using Greet;
using Greet.Services;


public static class StartCommand
{
    public static Command Execute()
    {
        var portOption = new Option<int>(
            "--port",
            description: "the addr to serve the api on",
            getDefaultValue: () => 50051);
        var jsonLogOption = new Option<bool>(
            "--json-log",
            description: "The format of the log is json or user friendly key-value pairs",
            getDefaultValue: () => false);
        var remoteAddrOption = new Option<string>(
            "--remote-addr",
            description: "the demo server's addr for this server to connect to",
            getDefaultValue: () => string.Empty);
        var intervalMilliSecOption = new Option<long>(
            "--interval-milli-sec",
            description: "The interval between two requests. Negative numbers mean sending one request.",
            getDefaultValue: () => 0);

        var startCommand = new Command("start", "Start the service")
        {
            portOption,
            jsonLogOption,
            remoteAddrOption,
            intervalMilliSecOption
        };

        startCommand.Handler = CommandHandler.Create<ServerOptions>(async (options) => await Server.Serve(options));

        return startCommand;
    }
}
