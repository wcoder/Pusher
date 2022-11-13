namespace Pusher.APNs;

public class Payload
{
    private Payload(string data)
    {
        Data = data;
    }

    internal string Data { get; }

    public static Payload FromRawJson(string json)
    {
        return new Payload(json);
    }
}