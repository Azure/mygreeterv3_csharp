using System.Threading.Tasks;
using System.CommandLine;

using Grpc.Net.Client;
using Greet;


var rootCommand = new RootCommand("A brief description of your service");
rootCommand.AddCommand(StartCommand.Execute());

await rootCommand.InvokeAsync(args);