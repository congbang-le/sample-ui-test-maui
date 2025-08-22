namespace VisitTracker;

public partial class TaskDetailPage : BaseContentPage<TaskDetailVm>
{
    public TaskDetailPage(TaskDetailVm viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
    }
}