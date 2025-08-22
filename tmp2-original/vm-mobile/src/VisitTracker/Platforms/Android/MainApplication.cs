﻿using Android.App;
using Android.Runtime;

namespace VisitTracker;

[Application(UsesCleartextTraffic = true,
    Icon = "@mipmap/appicon",
    RoundIcon = "@mipmap/appicon_round")]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    { }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}