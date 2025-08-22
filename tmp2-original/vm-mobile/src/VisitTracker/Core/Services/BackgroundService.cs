namespace VisitTracker;

/// <summary>
/// BackgroundService is a class that manages a background timer for syncing data in the application.
/// It uses a Timer to periodically check for unsynced data and sync it with the server.
/// </summary>
public class BackgroundService
{
    private Timer BackgroundTimer;

    public BackgroundService()
    {
        BackgroundTimer = new Timer();
    }

    /// <summary>
    /// StartBackgroundTimer starts a background timer that checks for unsynced data and syncs it with the server.
    /// It uses a Timer to periodically check for unsynced data and sync it with the server.
    /// </summary>
    /// <param name="timeToRepeat"></param>
    /// <param name="intervalInSecs"></param>
    /// <returns></returns>
    public async Task StartBackgroundTimer(int timeToRepeat, int intervalInSecs)
    {
        StopBackgroundTimer();

        if (BackgroundTimer == null)
            BackgroundTimer = new Timer();

        BackgroundTimer.Interval = intervalInSecs * 1000;
        BackgroundTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            MainThread.InvokeOnMainThreadAsync(async () =>
             {
                 var syncData = await AppServices.Current.SyncService.GetAllUnSynced();
                 if (syncData != null && syncData.Any())
                     await AppServices.Current.SyncService.SyncData();

                 var syncDataAgain = await AppServices.Current.SyncService.GetAllUnSynced();
                 if (syncDataAgain == null || !syncDataAgain.Any())
                 {
                     StopBackgroundTimer();

                     var ongoingVm = ServiceLocator.GetService<OngoingVm>();
                     if (ongoingVm != null && syncData != null
                            && syncData.Any(x => x.IdentifierId == ongoingVm.OngoingDto?.BookingDetail?.Id))
                         ongoingVm.RefreshOnAppear = true;
                 }

                 timeToRepeat--;
                 if (timeToRepeat <= 0)
                     StopBackgroundTimer();
             }
         );
        BackgroundTimer.Start();

        await Task.CompletedTask;
    }

    /// <summary>
    /// StopBackgroundTimer stops the background timer that checks for unsynced data after it syncs with the server.
    /// It stops the timer and releases any resources associated with it.
    /// </summary>
    public void StopBackgroundTimer()
    {
        if (BackgroundTimer != null)
            BackgroundTimer.Stop();
    }
}