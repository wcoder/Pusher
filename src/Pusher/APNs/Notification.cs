using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Pusher.APNs;

/// <summary>
/// https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/sending_notification_requests_to_apns?language=objc
/// </summary>
public class Notification
{
    private readonly string _authToken;
    private string? _apiBaseUrl;
    private string? _deviceToken;
    private string? _payload;
    private string? _apnsTopic;
    private string? _apnsPriority;
    private string? _apnsPushType;
    private string? _apnsExpiration;
    private string? _apnsCollapseId;
    private string? _apnsId;

    private Notification(string authToken)
    {
        _authToken = authToken;
    }

    public static Notification WithP8Key(P8Key key)
    {
        return new Notification(key.Token);
    }

    public Notification WithEnvironment(Env env)
    {
        _apiBaseUrl = env switch
        {
            Env.Development => "https://api.sandbox.push.apple.com/3/device",
            Env.Production => "https://api.push.apple.com/3/device",
            _ => throw new Exception("undefined"),
        };
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="deviceToken">
    /// The device push token it's the hexadecimal bytes that identify the user’s device.
    /// Your app receives the bytes for this device token when registering for remote notifications.
    /// </param>
    /// <returns></returns>
    public Notification WithDeviceToken(string deviceToken)
    {
        _deviceToken = deviceToken;
        return this;
    }
    
    /// <summary>
    /// </summary>
    /// <param name="bundleId">
    /// The topic for the notification. In general, the topic is your app’s bundle ID/app ID.
    /// It can have a suffix based on the type of push notification.
    /// </param>
    /// <returns></returns>
    public Notification WithTopic(string bundleId)
    {
        _apnsTopic = bundleId;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="priority">
    /// The priority of the notification. If you omit this, APNs sets the notification priority to 10.
    /// </param>
    /// <returns></returns>
    public Notification WithPriority(Priority priority)
    {
        _apnsPriority = priority.ToString();
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="expirationDate">
    /// The date at which the notification is no longer valid. This value is a UNIX epoch expressed in seconds (UTC).
    /// If the value is nonzero, APNs stores the notification and tries to deliver it at least once,
    /// repeating the attempt as needed until the specified date.
    /// If the value is 0, APNs attempts to deliver the notification only once and doesn't store it.
    /// </param>
    /// <returns></returns>
    public Notification WithExpiration(DateTime expirationDate) // TODO: check
    {
        _apnsExpiration = new DateTimeOffset(expirationDate).ToUnixTimeSeconds().ToString();
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="payload">
    /// JSON payload with the notification’s content into the body of your request.
    /// The JSON payload must not be compressed and is limited to a maximum size of 4 KB (4096 bytes).
    /// For a Voice over Internet Protocol (VoIP) notification, the maximum size is 5 KB (5120 bytes).
    /// </param>
    /// <returns></returns>
    public Notification WithPayload(Payload payload)
    {
        _payload = payload.Data;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="type">
    /// Must accurately reflect the contents of your notification’s payload. If there’s a mismatch,
    /// or if the header is missing on required systems, APNs may return an error,
    /// delay the delivery of the notification, or drop it altogether.
    /// </param>
    /// <returns></returns>
    public Notification WithType(PushType type)
    {
        _apnsPushType = type.ToString().ToLower();
        return this;
    }

    public Notification WithCollapseId(string id)
    {
        _apnsCollapseId = id;
        return this;
    }

    public Notification WithId(string id)
    {
        _apnsId = id;
        return this;
    }

    public async Task<string> Send() // TODO: Export
    {
        ArgumentException.ThrowIfNullOrEmpty(_payload, "Payload can't be empty");

        var url = $"{_apiBaseUrl}/{_deviceToken}";

        using var client = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Version = new Version(2, 0),
            Headers = {
                { $"{HttpRequestHeader.Authorization}", new AuthenticationHeaderValue("Bearer", _authToken).ToString() },
            },
            Content = new StringContent(_payload, Encoding.UTF8, "application/json"),
        };
        if (!string.IsNullOrEmpty(_apnsTopic))
        {
            request.Headers.Add("apns-topic", _apnsTopic);
        }
        if (!string.IsNullOrEmpty(_apnsPriority))
        {
            request.Headers.Add("apns-priority", _apnsPriority);
        }
        if (!string.IsNullOrEmpty(_apnsPushType))
        {
            request.Headers.Add("apns-push-type", _apnsPushType);
        }
        if (!string.IsNullOrEmpty(_apnsExpiration))
        {
            request.Headers.Add("apns-expiration", _apnsExpiration);
        }
        if (!string.IsNullOrEmpty(_apnsCollapseId))
        {
            request.Headers.Add("apns-collapse-id", _apnsCollapseId);
        }
        if (!string.IsNullOrEmpty(_apnsId))
        {
            request.Headers.Add("apns-id", _apnsId);
        }
        Debug.WriteLine(request);
        Debug.WriteLine($"Payload: {_payload}");

        var response = await client.SendAsync(request);
        Debug.WriteLine(response);

        // response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();

        return data;
    }
}