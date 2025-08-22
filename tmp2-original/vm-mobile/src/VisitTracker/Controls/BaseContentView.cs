namespace VisitTracker;

/// <summary>
/// BaseContentView is a base class for all content views in the application.
/// It provides a common implementation for the IsBusy property, which can be used to indicate whether the view is busy or not.
/// </summary>
public class BaseContentView : ContentView
{
    [Reactive] public bool IsBusy { get; set; }
    [Reactive] public bool IsNotBusy => !IsBusy;

    protected void BindBusy(IReactiveCommand command)
    {
        command.IsExecuting.Subscribe(
                x => this.IsBusy = x,
                _ => this.IsBusy = false,
                () => this.IsBusy = false
            );
    }
}