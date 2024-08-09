#if SERVER

using System.CommandLine;
using System.Threading.Tasks;

using ServiceHub.MyGreeter;
using Server;

var rootCommand = new RootCommand("A brief description of your service");
rootCommand.AddCommand(StartCommand.Execute());
await rootCommand.InvokeAsync(args);

#endif