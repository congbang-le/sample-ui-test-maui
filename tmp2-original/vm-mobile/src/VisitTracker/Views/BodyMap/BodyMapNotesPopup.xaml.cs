namespace VisitTracker;

public partial class BodyMapNotesPopup : Popup
{
    private BodyMapNotesPopupVm vm = new BodyMapNotesPopupVm();

    public BodyMapNotesPopup(BodyMapNotesPopupVm csharpBindingPopupViewModel)
    {
        InitializeComponent();
        BindingContext = vm = csharpBindingPopupViewModel;
    }

    public void Button_Clicked(object sender, System.EventArgs e)
    {
        Close(BindingContext);
    }

    public void Save_Clicked(object sender, System.EventArgs e)
    {
        vm.IsAllowToSave = true;
        Close(vm);
    }

    public void Discard_Clicked(object sender, System.EventArgs e)
    {
        vm.IsAllowToDiscard = true;
        Close(vm);
    }
}