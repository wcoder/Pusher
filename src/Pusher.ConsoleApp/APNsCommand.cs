using System.CommandLine;

namespace Pusher.ConsoleApp;

internal class APNsCommand : Command
{
    public APNsCommand()
        : base("apns", "Use APNs provider")
    {
        var p8Command = new P8Command();
        AddCommand(p8Command);
    }
}
