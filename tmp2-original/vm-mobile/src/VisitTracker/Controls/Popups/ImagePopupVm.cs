using System.Windows.Input;

namespace VisitTracker;

public class ImagePopupVm : ReactiveObject
{
    private readonly Popup _popup;

    public ImageSource ImageSource { get; }

    public ICommand CloseCommand { get; }

    public ImagePopupVm(Popup popup, ImageSource imageSource)
    {
        _popup = popup;
        ImageSource = imageSource;
        CloseCommand = new Command(ClosePopup);
    }

    private void ClosePopup()
    {
        _popup.Close();
    }
}