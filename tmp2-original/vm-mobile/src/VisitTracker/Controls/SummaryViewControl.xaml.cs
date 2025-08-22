namespace VisitTracker;

public partial class SummaryViewControl : BaseContentView
{
    public ReactiveCommand<AttachmentDto, Unit> OpenAttachmentCommand { get; set; }

    public SummaryViewControl()
    {
        InitializeComponent();

        OpenAttachmentCommand = ReactiveCommand.CreateFromTask<AttachmentDto>(OpenAttachment);
        BindBusy(OpenAttachmentCommand);
    }

    protected async Task OpenAttachment(AttachmentDto attachment) => await Launcher.Default.OpenAsync(new OpenFileRequest("Open File", new ReadOnlyFile(attachment.FilePath.ToString())));

    public static readonly BindableProperty MaxCharsProperty = BindableProperty.Create(nameof(MaxChars),
            typeof(int), typeof(SummaryViewControl), defaultValue: 2000);

    public static readonly BindableProperty EnableSummaryTemplateProperty = BindableProperty.Create(nameof(EnableSummaryTemplate),
            typeof(bool), typeof(SummaryViewControl), defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: EnableSummaryTemplateChanged);

    public static readonly BindableProperty SummaryProperty = BindableProperty.Create(nameof(Summary),
            typeof(string), typeof(SummaryViewControl), defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay, propertyChanged: SummaryPropertyChanged);

    public static readonly BindableProperty AttachmentListProperty = BindableProperty.Create(nameof(AttachmentList),
            typeof(IList<AttachmentDto>), typeof(AttachmentControl),
            defaultValue: new List<AttachmentDto>(), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: AttachmentListPropertyChanged);

    public static readonly BindableProperty VisibleAttachmentControlProperty = BindableProperty.Create(nameof(VisibleAttachmentControl),
            typeof(bool), typeof(SummaryViewControl),
            defaultValue: false, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: VisibleAttachmentControlPropertyChanged);

    private static void EnableSummaryTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SummaryViewControl)bindable;
        control.EnableSummaryTemplate = (bool)newValue;
    }

    private static void SummaryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SummaryViewControl)bindable;
        if (newValue == null)
            control.Summary = "";
        else control.Summary = newValue?.ToString();
    }

    private static void AttachmentListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SummaryViewControl)bindable;
        control.AttachmentList = newValue as IList<AttachmentDto>;

        control.IsAttachmentEnableContent();
    }

    private static void VisibleAttachmentControlPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SummaryViewControl)bindable;
        control.VisibleAttachmentControl = (bool)newValue;
    }

    private void IsAttachmentEnableContent()
    {
        VisibleAttachmentControl = (AttachmentList != null && AttachmentList.Count() > 0 && AttachmentList.Any());
    }

    public bool EnableSummaryTemplate
    {
        get
        {
            return (bool)base.GetValue(EnableSummaryTemplateProperty);
        }
        set
        {
            base.SetValue(EnableSummaryTemplateProperty, value);
        }
    }

    public string Summary
    {
        get
        {
            return base.GetValue(SummaryProperty)?.ToString();
        }
        set
        {
            base.SetValue(SummaryProperty, value);
        }
    }

    public int MaxChars
    {
        get
        {
            return (int)base.GetValue(MaxCharsProperty);
        }
        set
        {
            base.SetValue(MaxCharsProperty, value);
        }
    }

    public bool VisibleAttachmentControl
    {
        get
        {
            return (bool)GetValue(VisibleAttachmentControlProperty);
        }
        private set
        {
            SetValue(VisibleAttachmentControlProperty, value);
        }
    }

    public IList<AttachmentDto> AttachmentList
    {
        get
        {
            return base.GetValue(AttachmentListProperty) as IList<AttachmentDto>;
        }
        set
        {
            base.SetValue(AttachmentListProperty, value);
        }
    }
}