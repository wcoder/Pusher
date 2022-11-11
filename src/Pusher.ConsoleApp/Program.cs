using Pusher;
using Pusher.APNs;

var keyPath = "";
var teamId = "";
var keyId = "";

var bundleId = "";
var deviceId = "";
var payload = $$"""
{
    "aps" : {
        "alert" : {
            "title" : "Push On Simulator",
            "body" : "You have sent it on simulator"
        },
        "badge" : 1
    }
}
""";

var result = await Notification
    .WithP8Key(await P8Key.FromFile(keyPath, teamId, keyId))
    .WithEnvironment(Env.Development)
    .WithTopic(bundleId)
    .WithDeviceToken(deviceId)
    .WithPriority(Priority.Immediately)
    .WithPayload(Payload.FromRawJson(payload))
    .WithExpiration(DateTime.UtcNow.AddSeconds(10))
    .WithType(PushType.Alert)
    .Send();

Console.WriteLine(result);


Console.WriteLine("Done");
Console.ReadKey();

