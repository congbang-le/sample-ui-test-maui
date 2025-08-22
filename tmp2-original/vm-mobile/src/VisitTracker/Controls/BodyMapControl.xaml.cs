namespace VisitTracker;

public partial class BodyMapControl : BaseContentView
{
    public ReactiveCommand<BodyMap, Unit> OpenDeleteConfirmationCommand { get; set; }
    public ReactiveCommand<BodyMap, Unit> OpenByBodyMapCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenBodyMapCommand { get; set; }

    public BodyMapControl()
    {
        InitializeComponent();

        OpenDeleteConfirmationCommand = ReactiveCommand.CreateFromTask<BodyMap>(OpenDeleteConfirmation);
        OpenByBodyMapCommand = ReactiveCommand.CreateFromTask<BodyMap>(OpenByBodyMap);
        OpenBodyMapCommand = ReactiveCommand.CreateFromTask(OpenBodyMap);

        BindBusy(OpenDeleteConfirmationCommand);
        BindBusy(OpenByBodyMapCommand);
        BindBusy(OpenBodyMapCommand);
    }

    public static readonly BindableProperty BodyMapListProperty = BindableProperty.Create(nameof(BodyMapList),
            typeof(IList<BodyMap>), typeof(BodyMapControl),
            defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BodyMapListPropertyChanged);

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly),
            typeof(bool), typeof(BodyMapControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsReadOnlyPropertyChanged);

    public static readonly BindableProperty BodyMapIdProperty = BindableProperty.Create(nameof(BodyMapId),
            typeof(int), typeof(BodyMapControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BodyMapIdPropertyChanged);

    public static readonly BindableProperty BaseVisitIdProperty = BindableProperty.Create(nameof(BaseVisitId),
            typeof(int), typeof(BodyMapControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: BaseVisitIdPropertyChanged);

    public static readonly BindableProperty TypeIdProperty = BindableProperty.Create(nameof(TypeId),
            typeof(int), typeof(BodyMapControl), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TypeIdPropertyChanged);

    public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type),
            typeof(string), typeof(BodyMapControl),
            defaultValue: null, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: TypePropertyChanged);

    public static void TypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BodyMapControl)bindable;
        control.Type = newValue as string;
    }

    private static void TypeIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BodyMapControl)bindable;
        control.TypeId = Convert.ToInt32(newValue);
    }

    private static void BaseVisitIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BodyMapControl)bindable;
        control.BaseVisitId = Convert.ToInt32(newValue);
    }

    private static void BodyMapIdPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BodyMapControl)bindable;
        control.BodyMapId = Convert.ToInt32(newValue);
    }

    public static void BodyMapListPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BodyMapControl)bindable;
        control.BodyMapList = newValue as IList<BodyMap>;
    }

    private static void IsReadOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BodyMapControl)bindable;
        control.IsReadOnly = (bool)newValue;
    }

    public IList<BodyMap> BodyMapList
    {
        get
        {
            return base.GetValue(BodyMapListProperty) as IList<BodyMap>;
        }
        set
        {
            base.SetValue(BodyMapListProperty, value);
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return (bool)GetValue(IsReadOnlyProperty);
        }
        set
        {
            SetValue(IsReadOnlyProperty, value);
        }
    }

    public int BodyMapId
    {
        get
        {
            return Convert.ToInt32(GetValue(BodyMapIdProperty));
        }
        set
        {
            SetValue(BodyMapIdProperty, value);
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

    public async Task OpenDeleteConfirmation(BodyMap bodyMap)
    {
        if (bodyMap == null)
            return;

        bool answer = await App.Current.MainPage.DisplayAlert(Messages.DialogConfirmationTitle, Messages.DialogDeleteConfirmation, Messages.Yes, Messages.No);
        if (answer)
        {
            await AppServices.Current.AttachmentService.DeleteAllByBodyMapId(bodyMap.Id);

            await AppServices.Current.BodyMapService.DeleteAllById(bodyMap.Id);
            BodyMapList.Remove(bodyMap);

            BodyMapList = BodyMapList?.OrderBy(i => i.AddedOn)
                          .ToList();

            await Application.Current.MainPage.ShowSnackbar(Messages.DeleteSuccess, true);
        }
    }

    protected async Task OpenByBodyMap(BodyMap bodyMap)
    {
        var navigationParameter = new Dictionary<string, object>
            {
                { nameof(BodyMap), bodyMap },
                { nameof(BaseVisitId), BaseVisitId },
                { nameof(TypeId), TypeId },
                { nameof(Type), Type },
                { nameof(IsReadOnly), IsReadOnly }
            };

        var bodymapPage = $"{nameof(BodyMapPage)}?";
        await Shell.Current.GoToAsync(bodymapPage, navigationParameter);
    }

    protected async Task OpenBodyMap()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            {nameof(BodyMap), null },
            {nameof(BaseVisitId), BaseVisitId },
            {nameof(TypeId), TypeId },
            {nameof(Type), Type },
            {nameof(IsReadOnly), IsReadOnly }
        };

        var bodymapPage = $"{nameof(BodyMapPage)}?";
        await Shell.Current.GoToAsync(bodymapPage, navigationParameter);
    }
}