using System.CommandLine;
using Pusher.ConsoleApp;

var rootCommand = new RootCommand("Pusher CLI description.")
{
    new APNsCommand(),
};

await rootCommand.InvokeAsync(args);

return 0;