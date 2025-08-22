using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace VisitTracker;

[Activity(Name = "com.artivis.vm.activity",
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleInstance,
    AlwaysRetainTaskState = true,
    AutoRemoveFromRecents = false,
    ResumeWhilePausing = true,
    WindowSoftInputMode = Android.Views.SoftInput.AdjustResize,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Density |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    private CareWorkerLocationServiceConnection CareWorkerLocationServiceConnection;
    private SupervisorLocationServiceConnection SupervisorLocationServiceConnection;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        CareWorkerLocationServiceConnection = new CareWorkerLocationServiceConnection();
        SupervisorLocationServiceConnection = new SupervisorLocationServiceConnection();
        //_ = GetRemoteConfig();

        CheckFromNotification(Intent);

        // Force the app to always display in light mode
        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        CheckFromNotification(intent);
        //PushNotificationManager.ProcessIntent(this, intent);
    }

    protected override void OnStart()
    {
        base.OnStart();

        BindService(new Intent(this, typeof(CareWorkerLocationService)), CareWorkerLocationServiceConnection, Bind.AutoCreate);
        BindService(new Intent(this, typeof(SupervisorLocationService)), SupervisorLocationServiceConnection, Bind.AutoCreate);
    }

    protected override void OnStop()
    {
        if (App.LocationUpdatesServiceBound)
        {
            UnbindService(CareWorkerLocationServiceConnection);
            UnbindService(SupervisorLocationServiceConnection);

            App.LocationUpdatesServiceBound = false;
        }

        base.OnStop();
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        WeakReferenceMessenger.Default.Send(new MessagingEvents.PermissionsUpdatedMessage(true));
    }

    private void CheckFromNotification(Intent intent)
    {
        if (intent == null || intent.Extras == null)
            return;

        if (intent.HasExtra(nameof(Domain.Notification)))
        {
            var content = intent.Extras.GetString(nameof(Domain.Notification));
            if (string.IsNullOrEmpty(content)) return;

            var dbNotification = JsonExtensions.Deserialize<Domain.Notification>(content);
            WeakReferenceMessenger.Default.Send(new MessagingEvents.NotificationMessage(dbNotification.Id));
        }
    }
}