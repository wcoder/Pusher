using System.ComponentModel;
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

    public Notification WithDeviceToken(string deviceToken)
    {
        _deviceToken = deviceToken;
        return this;
    }

    public Notification WithTopic(string bundleId)
    {
        _apnsTopic = bundleId;
        return this;
    }


    public Notification WithPriority(Priority priority)
    {
        _apnsPriority = priority.ToString();
        return this;
    }

    public Notification WithExpiration(DateTime expirationDate) // TODO: check
    {
        _apnsExpiration = new DateTimeOffset(expirationDate).ToUnixTimeSeconds().ToString();
        return this;
    }

    public Notification WithPayload(Payload payload)
    {
        _payload = payload.Data;
        return this;
    }

    public Notification WithType(PushType type)
    {
        _apnsPushType = type switch
        {
            PushType.Alert => "alert",
            PushType.Background => "background",
            _ => throw new InvalidEnumArgumentException("undefined type")
        };
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