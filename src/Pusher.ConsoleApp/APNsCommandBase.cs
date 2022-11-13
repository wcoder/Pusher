using System.CommandLine;
using Pusher.APNs;

namespace Pusher.ConsoleApp;

internal abstract class APNsCommandBase : Command
{
    protected APNsCommandBase(string name, string? description = null)
        : base(name, description)
    {
        TopicOption = new Option<string>(
            "--topic",
            "The topic for the notification. In general, the topic is your app’s bundle ID/app ID.")
        {
            IsRequired = true,
            ArgumentHelpName = "com.example.ios-app",
        };
        Add(TopicOption);

        DeviceTokenOption = new Option<string>(
            "--device",
            "The device push token it's the hexadecimal bytes that identify the user’s device. " +
            "Your app receives the bytes for this device token when registering for remote notifications.")
        {
            IsRequired = true,
            ArgumentHelpName = "XXXX...XXX",
        };
        Add(DeviceTokenOption);

        PayloadOption = new Option<FileInfo>(
            "--payload",
            "JSON payload with the notification’s content.")
        {
            IsRequired = true,
            ArgumentHelpName = "FILEPATH",
        };
        Add(PayloadOption);

        PriorityOption = new Option<byte>("--priority", "Priority")
        {
            ArgumentHelpName = "1-10",
        };
        PriorityOption.SetDefaultValue(10);
        Add(PriorityOption);

        PushTypeOption = new Option<PushType>("--type", "Push type");
        PushTypeOption.SetDefaultValue(PushType.Alert);
        Add(PushTypeOption);

        ProductionOption = new Option<bool>(
            new[] {"--production", "-prod"},
            "Use production APNs API or Sandbox");
        ProductionOption.SetDefaultValue(false);
        Add(ProductionOption);
    }

    protected Option<PushType> PushTypeOption { get; }
    protected Option<bool> ProductionOption { get; }
    protected Option<string> TopicOption { get; }
    protected Option<string> DeviceTokenOption { get; }
    protected Option<FileInfo> PayloadOption { get; }
    protected Option<byte> PriorityOption { get; }
}