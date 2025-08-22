namespace VisitTracker;

public partial class HorizontalAttachmentControl : BaseContentView
{
    #region Commands & Constructor

    public ReactiveCommand<AttachmentDto, Unit> OpenAttachmentCommand { get; set; }

    public HorizontalAttachmentControl()
    {
        InitializeComponent();

        OpenAttachmentCommand = ReactiveCommand.CreateFromTask<AttachmentDto>(OpenAttachment);
        BindBusy(OpenAttachmentCommand);
    }

    #endregion Commands & Constructor

    #region Bindable Properties

    public static readonly BindableProperty AttachmentListProperty = BindableProperty.Create(nameof(AttachmentList),
            typeof(IList<AttachmentDto>), typeof(HorizontalAttachmentControl),
            defaultValue: new List<AttachmentDto>(), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: AttachmentListPropertyChanged);

    public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type),
            typeof(string), typeof(HorizontalAttachmentControl),
            defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TypePropertyChanged);

    public static readonly BindableProperty BaseVisitIdProperty = BindableProperty.Create(nameof(BaseVisitId),
            typeof(int), typeof(HorizontalAttachmentControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BaseVisitIdPropertyChanged);

    public static readonly BindableProperty TypeIdProperty = BindableProperty.Create(nameof(TypeId),
            typeof(int), typeof(HorizontalAttachmentControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TypeIdPropertyChanged);

    public static void TypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (HorizontalAttachmentControl)bindable;
        control.Type = newValue as string;
    }

    private static void BaseVisitIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (HorizontalAttachmentControl)bindable;
        control.BaseVisitId = Convert.ToInt32(newValue);
    }

    private static void TypeIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (HorizontalAttachmentControl)bindable;
        control.TypeId = Convert.ToInt32(newValue);
    }

    private static void AttachmentListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (HorizontalAttachmentControl)bindable;
        if (newValue is IList<AttachmentDto> attachments)
        {
            foreach (var attachment in attachments)
            {
                attachment.DisplayIcon = attachment.AttachmentType == EAttachmentType.Image
                    ? MaterialCommunityIconsFont.Image
                    : MaterialCommunityIconsFont.PlayCircle;
            }

            control.AttachmentList = attachments;
        }
    }

    #endregion Bindable Properties

    #region Properties

    public IList<AttachmentDto> AttachmentList
    {
        get
        {
            return base.GetValue(AttachmentListProperty) as List<AttachmentDto>;
        }
        set
        {
            base.SetValue(AttachmentListProperty, value);
        }
    }

    public int BaseVisitId
    {
        get
        {
            return Convert.ToInt32(GetValue(BaseVisitIdProperty));
        }
        set
        {
            SetValue(BaseVisitIdProperty, value);
        }
    }

    public int TypeId
    {
        get
        {
            return Convert.ToInt32(GetValue(TypeIdProperty));
        }
        set
        {
            SetValue(TypeIdProperty, value);
        }
    }

    public string Type
    {
        get
        {
            return base.GetValue(TypeProperty) as string;
        }
        set
        {
            base.SetValue(TypeProperty, value);
        }
    }

    #endregion Properties

    #region Events & Functions

    protected async Task OpenAttachment(AttachmentDto attachment) => await AttachmentHelper.Current.OpenAttachment(attachment, AttachmentList);

    #endregion Events & Functions
}