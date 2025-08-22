namespace VisitTracker;

public partial class ImagePopup : Popup
{
    public ImagePopup(ImageSource imageSource)
    {
        InitializeComponent();
        BindingContext = new ImagePopupVm(this, imageSource);
    }
}