namespace VisitTracker;

public class LoginProviderQrScannerVm : BaseVm
{
    [Reactive] public string AccessCode { get; set; }
    [Reactive] public BarcodeReaderOptions QrReaderOptions { get; set; }

    public ReactiveCommand<BarcodeDetectionEventArgs, Unit> OnBarcodeDetectedCommand { get; }

    public LoginProviderQrScannerVm()
    {
        OnBarcodeDetectedCommand = ReactiveCommand.CreateFromTask<BarcodeDetectionEventArgs>(OnBarcodeDetected);

        BindBusyWithException(OnBarcodeDetectedCommand);

        QrReaderOptions = new BarcodeReaderOptions
        {
            AutoRotate = true,
            Multiple = false,
            Formats = BarcodeFormat.QrCode
        };
    }

    protected override async Task Init() => await Task.Delay(200);

    protected async Task OnBarcodeDetected(BarcodeDetectionEventArgs args)
    {
        if (args?.Results != null && args.Results.Any())
        {
            AccessCode = args.Results.FirstOrDefault()?.Value;
            WeakReferenceMessenger.Default.Send(new MessagingEvents.QrProviderCodeMessage(AccessCode));

            await MainThread.InvokeOnMainThreadAsync(App.Current.MainPage.Navigation.PopToRootAsync);
        }
    }
}