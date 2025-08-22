namespace VisitTracker;

/// <summary>
/// SnackbarExtensions is a static class that provides extension methods for displaying snackbars in the application.
/// It includes methods for showing success and failure messages with customizable options such as duration and appearance.
/// </summary>
public static class SnackbarExtensions
{
    private static SnackbarOptions failureSnackbarOptions = new()
    {
        BackgroundColor = Color.FromArgb("#FF794F"),
        TextColor = Colors.WhiteSmoke,
        ActionButtonTextColor = Colors.White,
        CornerRadius = new CornerRadius(6),
        Font =  Microsoft.Maui.Font.SystemFontOfSize(16)
    };

    private static ISnackbar snackbar;

    private static SnackbarOptions successSnackbarOptions = new()
    {
        BackgroundColor = Color.FromArgb("#00BE95"),
        TextColor = Colors.WhiteSmoke,
        ActionButtonTextColor = Colors.White,
        CornerRadius = new CornerRadius(6),
        Font = Microsoft.Maui.Font.SystemFontOfSize(16)
    };

    /// <summary>
    /// ShowSnackbar is an extension method for the Page class that displays a snackbar message with customizable options.
    /// It uses the Snackbar class to create and show the snackbar with the specified message, success status, and duration.
    /// </summary>
    /// <param name="page">Uses this page to invoke the function</param>
    /// <param name="message">Message to be disaplyed</param>
    /// <param name="isSuccess">True for Success - Success/Failure message to derive color</param>
    /// <param name="isLonger">True for Longer - Longer/Shorter duration of the message</param>
    /// <returns></returns>
    public static async Task ShowSnackbar(this Page page, string message, bool isSuccess, bool isLonger = false)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            snackbar = Snackbar.Make(
                message,
                async () => { await snackbar.Dismiss(); },
                "",
                TimeSpan.FromSeconds(
                    (isSuccess ? Constants.SnackbarSuccessTimeInSecs : Constants.SnackbarFailureTimeInSecs)
                    * (isLonger ? Constants.SnackbarLongerByMultiplication : 1)
                ),
                isSuccess ? successSnackbarOptions : failureSnackbarOptions);
            await snackbar.Show();
        });
    }
}