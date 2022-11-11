namespace Pusher.APNs;

public class Priority
{
    private readonly byte _priority;

    private Priority(byte priority)
    {
        _priority = priority;
    }
    
    public static Priority PowerSafe = new(1);
    public static Priority PowerAuto = new(5);
    public static Priority Immediately = new(10);

    public override string ToString()
    {
        return _priority.ToString();
    }
}