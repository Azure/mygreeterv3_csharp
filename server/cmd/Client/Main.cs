#if CLIENT

using System.Threading.Tasks;
using System.CommandLine;

using Grpc.Net.Client;
using ServiceHub.MyGreeter;
using Client;

var rootCommand = new RootCommand("A brief description of your service");
rootCommand.AddCommand(StartCommand.Execute());
await rootCommand.InvokeAsync(args);

#endif