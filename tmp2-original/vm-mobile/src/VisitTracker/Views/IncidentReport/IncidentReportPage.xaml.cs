namespace VisitTracker;

public partial class IncidentReportPage : BaseContentPage<IncidentReportVm>
{
    public IncidentReportPage(IncidentReportVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}