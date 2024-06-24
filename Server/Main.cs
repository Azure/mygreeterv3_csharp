using System.CommandLine;
using System.Threading.Tasks;

using Greet;
using Greet.Services;


var rootCommand = new RootCommand("A brief description of your service");
rootCommand.AddCommand(StartCommand.Execute());

await rootCommand.InvokeAsync(args);
