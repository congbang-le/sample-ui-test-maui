namespace VisitTracker;

public partial class MedicationDetailPage : BaseContentPage<MedicationDetailVm>
{
    public MedicationDetailPage(MedicationDetailVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}