#if CLIENT

using System.Threading.Tasks;
using System.CommandLine;

using Grpc.Net.Client;
using ServiceHub.MyGreeterCsharp;
using Client;

var rootCommand = new RootCommand("This sample service demonstrates client-server communication using gRPC and shows how to access and interact with the Azure SDK");
rootCommand.AddCommand(StartCommand.Execute());
await rootCommand.InvokeAsync(args);

#endif