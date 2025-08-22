using System.Windows.Input;

namespace VisitTracker;

/// <summary>
/// Base ViewModel class for all ViewModels in the application.
/// It provides common properties and methods for handling exceptions, commands, and UI state.
/// </summary>
public abstract class BaseVm : ReactiveObject
{
    private CompositeDisposable deactivateWith;
    protected CompositeDisposable DeactivateWith => this.deactivateWith ??= new CompositeDisposable();

    protected virtual void Deactivate()
    {
        this.deactivateWith?.Dispose();
        this.deactivateWith = null;
    }

    public ICommand Command { get; set; }

    /// <summary>
    /// The title of the page, used for navigation and display purposes.
    /// </summary>
    [Reactive] public string Title { get; set; }

    /// <summary>
    /// The subtitle of the page, used for navigation and display purposes.
    /// </summary>
    [Reactive] public bool IsBusy { get; set; }
    public bool IsNotBusy => !IsBusy;

    public bool IsInputEnabled { get; set; }

    /// <summary>
    /// Indicates whether the page is in read-only mode.
    /// This property is used to control the editability of the page's content.
    /// </summary>
    [Reactive] public bool IsReadOnly { get; set; }
    public bool IsNotReadOnly => !IsReadOnly;

    /// <summary>
    /// Indicates whether the page should refresh its content when it appears.
    /// This property is used to control the behavior of the page when it is navigated to.
    /// </summary>
    [Reactive] public bool RefreshOnAppear { get; set; } = true;

    /// <summary>
    /// The command that is executed when the page is initialized.
    /// This command is used to perform any necessary setup or data loading when the page is displayed.
    /// </summary>
    public ReactiveCommand<bool, Unit> InitCommand { get; }

    /// <summary>
    /// Initializes the page and performs any necessary setup or data loading.
    /// This method is called when the page is displayed and is responsible for preparing the page's content.
    /// It should be overridden in derived classes to provide specific initialization logic.
    /// </summary>
    /// <returns></returns>
    protected abstract Task Init();

    protected BaseVm()
    {
        InitCommand = ReactiveCommand.CreateFromTask<bool>(
            async (isForced) =>
            {
                if (RefreshOnAppear || isForced)
                    await Init();
                RefreshOnAppear = false;
            }
        );
        BindBusyWithException(InitCommand);
    }

    /// <summary>
    /// 1. Binds the IsExecuting property of the command to the IsBusy property of the ViewModel.
    ///    This allows the UI to show a busy indicator when the command is executing.
    /// 2. It also subscribes to any exceptions thrown by the command and handles them appropriately.
    ///    The command's ThrownExceptions observable is used to handle exceptions that occur during command execution.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="disableBusy"></param>
    protected void BindBusyWithException(IReactiveCommand command, bool disableBusy = false)
    {
        if (!disableBusy)
            command.IsExecuting.Subscribe(
                    x => this.IsBusy = x,
                    _ => this.IsBusy = false,
                    () => this.IsBusy = false
                )
            .DisposeWith(this.DeactivateWith);

        command.ThrownExceptions.Subscribe(async e => await HandleException(e))
            .DisposeWith(this.DeactivateWith);
    }

    /// <summary>
    /// Binds the IsExecuting property of the command to the IsInputEnabled property of the ViewModel.
    /// This allows the UI to enable or disable input controls based on the command's execution state.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="inputObservable"></param>
    protected void BindInputDisabledWhileExecuting(IReactiveCommand command, IObservable<bool> inputObservable)
    {
        command.IsExecuting
            .CombineLatest(inputObservable, (isExecuting, isEnabled) => !isExecuting && isEnabled)
            .Subscribe(isEnabled =>
            {
                this.IsInputEnabled = isEnabled;
            })
            .DisposeWith(this.DeactivateWith);
    }

    /// <summary>
    /// Handles exceptions that occur during command execution.
    /// It tracks the error using the SystemHelper and displays an appropriate message to the user.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    protected async Task HandleException(Exception exception)
    {
        await SystemHelper.Current.TrackError(exception);

        if (exception is InvalidUserException)
        {
            if (Application.Current.MainPage is NavigationPage navPage)
                if (navPage.CurrentPage is LoginPage)
                    await Application.Current.MainPage.ShowSnackbar(Messages.LoginFailed, false);
                else if (navPage.CurrentPage is LoginProviderPage)
                    await Application.Current.MainPage.ShowSnackbar(Messages.LoginProviderFailed, false);
                else await SystemHelper.Current.Logout();
            else if (Application.Current.MainPage is LoginPage)
                await Application.Current.MainPage.ShowSnackbar(Messages.LoginFailed, false);
            else await SystemHelper.Current.Logout();
        }
        else if (exception is HttpRequestException)
            await Application.Current.MainPage.ShowSnackbar(Messages.ServerUnreachable, false);
        else if (exception is WebException)
            await Application.Current.MainPage.ShowSnackbar(Messages.ServerUnreachable, false);
        else
            await Application.Current.MainPage.ShowSnackbar(Messages.ExceptionOccurred, false);
    }

    /// <summary>
    /// Handles the back navigation for the page.
    /// This method is called when the user presses the back button or navigates back in the application.
    /// </summary>
    /// <returns></returns>
    protected async Task OnBack()
    {
        await Shell.Current.Navigation.PopAsync();
    }
}