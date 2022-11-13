using System.CommandLine;
using Pusher.APNs;

namespace Pusher.ConsoleApp;

internal class P8Command : APNsCommandBase
{
    public P8Command()
        : base("p8", "Use .p8 key")
    {
        var keyArgument = new Argument<string>("teamId:keyId:keyPath", ".p8 key info");
        keyArgument.AddValidator(result =>
        {
            var value = result.GetValueForArgument(keyArgument);
            if (string.IsNullOrEmpty(value))
            {
                result.ErrorMessage = "p8: Info is required";
            }
            else
            {
                var list = value.Split(":");
                if (list.Length != 3)
                {
                    result.ErrorMessage = "p8: Invalid format. Example: 'TEAMID:KEYID:/PATH/TO/KEY'";
                }
            }
        });
        AddArgument(keyArgument);

        this.SetHandler(HandleCommand, keyArgument, TopicOption, DeviceTokenOption, PayloadOption, PriorityOption, PushTypeOption, ProductionOption);
    }

    // TODO: refactor
    private async Task HandleCommand(
        string keyInfo,
        string topic,
        string deviceToken,
        FileInfo payloadFileInfo,
        byte priority,
        PushType type,
        bool production)
        {
            var (teamId, keyId, keyPath) = keyInfo.Split(':') switch
            {
                [var t, var i, var p] => (t, i, p),
                _ => default
            };
            Console.WriteLine($"Read key from '{keyPath}' ...");
            var p8Key = await P8Key.FromFile(keyPath, teamId, keyId);

            var payloadPath = payloadFileInfo.ToString();
            Console.WriteLine($"Read payload from '{payloadPath}' ...");
            var payload = await File.ReadAllTextAsync(payloadPath);

            Console.WriteLine($"Sending notification...");
            var result = await Notification
                .WithP8Key(p8Key)
                .WithEnvironment(production ? Env.Production : Env.Development)
                .WithTopic(topic)
                .WithDeviceToken(deviceToken)
                .WithPriority(new Priority(priority))
                .WithPayload(Payload.FromRawJson(payload))
                .WithType(type)
                // .WithExpiration(DateTime.UtcNow.AddSeconds(10))
                .Send();

            Console.WriteLine(string.IsNullOrEmpty(result) ? "Done" : result);
        }
}
