# Pusher

## Clients

- CLI (Preview)
- macOS (TODO)

## Features

- APNs
  - .p8

## Usage

### CLI

`cd Pusher.ConsoleApp`

Send alarm notification to sandbox:

```sh
dotnet run -- \
    apns p8 TEAMID:KEYID:/path/to/apns_key.p8 \
    --topic com.example.ios-app \
    --device C123...DEVICE0TOKENFE60...85883F9 \
    --payload /path/to/payload.apns
```

How to create `payload.apns`: [Generating a remote notification](https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification?language=objc)

## Release

### CLI

`cd Pusher.ConsoleApp`

Create a self-contained release for delivery and use:

OS | Command
---|--------
macOS Intel | `dotnet publish -c Release -r osx-x64 --self-contained`
macOS M1+ |  `dotnet publish -c Release -r osx-arm64 --self-contained`
Linux | `dotnet publish -c Release -r linux-x64 --self-contained`
Windows | `dotnet publish -c Release -r win-x64 --self-contained`

More RIDs: [.NET RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)


## Similar projects

- [XPusher](https://github.com/wcoder/XPusher)

---
© 2022 Yauheni Pakala | Apache License
