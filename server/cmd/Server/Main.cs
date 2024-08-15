#if SERVER

using System.CommandLine;
using System.Threading.Tasks;

using ServiceHub.MyGreeterCsharp;
using Server;

var rootCommand = new RootCommand("This sample service demonstrates client-server communication using gRPC and shows how to access and interact with the Azure SDK");
rootCommand.AddCommand(StartCommand.Execute());
await rootCommand.InvokeAsync(args);

#endif