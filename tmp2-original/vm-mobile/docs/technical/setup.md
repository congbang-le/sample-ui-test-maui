
### Requirements

- Visual Studio 2022 (Preferred) or Visual Studio Code
  - [Download Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
- .NET MAUI workload
- .NET 8 SDK (v8.0.406)
  - [Download for Windows](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.406-windows-x64-installer)
  - [Download for macOS](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.406-macos-x64-installer)

> **Note:** Development on macOS is not recommended for this project as it has limited support for .NET 8 MAUI development.

### Visual Studio Installation (Windows)

1. Download and run the Visual Studio 2022 installer from the link above
2. Select the following workloads:
   - Mobile development with .NET (includes .NET MAUI)
3. Complete the installation process
4. After installation, open Visual Studio and use the workload manager to ensure .NET MAUI is properly configured

After installing the .NET SDK, run:
```bash
dotnet workload restore
```

## Configuration

Firebase configuration files for push notifications are placed in following folders:
- Android: [google-services.json](../../src/VisitTracker/Platforms/Android/google-services.json)
- iOS: [GoogleService-Info.plist](../../src/VisitTracker/Platforms/iOS/GoogleService-Info.plist)