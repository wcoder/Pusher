namespace Pusher.APNs;

public class Payload
{
    private readonly string _data;

    internal string Data => _data;

    private Payload(string data)
    {
        _data = data;
    }

    public static Payload FromRawJson(string json)
    {
        return new Payload(json);
    }
}