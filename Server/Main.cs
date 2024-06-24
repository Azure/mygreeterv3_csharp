using System.CommandLine;
using System.Threading.Tasks;

using Greet;
using Greet.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("gRPC Service");
        rootCommand.AddCommand(StartCommand.Create());

        await rootCommand.InvokeAsync(args);
    }
}
