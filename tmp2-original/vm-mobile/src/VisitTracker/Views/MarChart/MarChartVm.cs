namespace VisitTracker;

[QueryProperty(nameof(Id), nameof(Id))]
public class MarChartVm : BaseVm, IQueryAttributable
{
    public int Id { get; set; }
    public List<MedicationsDto> Medications { get; set; }
    public ServiceUser ServiceUser { get; set; }
    public UserCardDto ServiceUserCard { get; set; }

    public ReactiveCommand<MedicationTimeAndDetailDto, Unit> HideWindowCommand { get; }
    public ReactiveCommand<MedicationTimeAndDetailDto, Unit> DateSelectedCommand { get; }

    public ReactiveCommand<Unit, Unit> OnSlideCommand { get; }
    public ReactiveCommand<Unit, Unit> GoTodayCommand { get; }
    public List<string> SliderDates { get; set; }
    public string SliderValueText { get; set; }
    public int SliderValue { get; set; }
    public int SliderMinValue { get; set; } = 1;
    public int SliderMaxValue { get; set; } = Constants.NoOfDaysMarChart;
    public bool IsVisibleNoDataControl { get; set; }

    public ReactiveCommand<Unit, Unit> OnBackCommand { get; }


    public MarChartVm()
    {
        HideWindowCommand = ReactiveCommand.CreateFromTask<MedicationTimeAndDetailDto>(HideWindow);
        DateSelectedCommand = ReactiveCommand.CreateFromTask<MedicationTimeAndDetailDto>(DateSelected);
        OnSlideCommand = ReactiveCommand.Create(OnSlide);
        GoTodayCommand = ReactiveCommand.Create(GoToday);
        OnBackCommand = ReactiveCommand.CreateFromTask(OnBack);

        BindBusyWithException(HideWindowCommand, true);
        BindBusyWithException(GoTodayCommand, true);
        BindBusyWithException(DateSelectedCommand, true);
        BindBusyWithException(OnSlideCommand, true);
        BindBusyWithException(OnBackCommand);
    }

    protected override async Task Init()
    {
        ServiceUser = await AppServices.Current.ServiceUserService.GetById(Id);
        ServiceUserCard = await BaseAssembler.BuildUserCardFromSu(ServiceUser, null);

        var response = await AppServices.Current.MarChartService.SyncMedicationHistoryByServiceUser(ServiceUser.Id);

        var dto = BaseAssembler.BuildMarChart(response.Medications, response.MedicationVisits);
        Medications = BaseAssembler.BuildMedicationsViewModel(dto);

        if (Medications != null && Medications.Count() > 0)
        {
            SliderDates = Medications.SelectMany(x => x.MedicationTimeAndDetailList.SelectMany(y =>
                            y.MedicationDetails.Select(z => z.Month + " " + z.Date))).Distinct().ToList();
            SliderValueText = SliderDates[SliderMaxValue - 1];
            SliderValue = SliderMinValue;
            IsVisibleNoDataControl = false;
        }
        else {
            IsVisibleNoDataControl = true;
        }
    }

    private async Task DateSelected(MedicationTimeAndDetailDto medication)
    {
        if (medication?.SelectedMedication != null)
        {
            medication.IsBusy = true;

            if (medication.PrevSelectedMedication != null)
                medication.PrevSelectedMedication.IsSelected = false;
            medication.SelectedMedication.IsSelected = true;
            medication.PrevSelectedMedication = medication.SelectedMedication;

            var medicationVisitNotAvailable = true;
            if (medication.SelectedMedication?.Id != 0)
            {
                var medicationDetail = await AppServices.Current.MarChartService.SyncMedicationHistoryByMedication(medication.SelectedMedication.Id);
                if (medicationDetail != null)
                {
                    medicationVisitNotAvailable = false;
                    medication.SelectedRecord = new MedicationHistoryDto()
                    {
                        Time = Convert.ToDateTime(medicationDetail.TimeSlot),
                        Completion = medicationDetail.Completion,
                        CompletionDetail = medicationDetail.CompletionDetail,
                        Summary = medicationDetail.Summary,
                        IsAvailable = true
                    };
                    medication.SelectedRecord.BackgroundColor = medication.SelectedMedication.BackgroundColor;
                }
            }

            if (medicationVisitNotAvailable)
            {
                Medications[0].MedicationName = "Medication";
                medication.SelectedRecord = null;
                medication.SelectedRecord = new MedicationHistoryDto { IsAvailable = false };
                medication.SelectedRecord.BackgroundColor = medication.SelectedMedication.BackgroundColor;
                medication.IsBusy = false;
            }

            medication.IsBusy = false;
        }

        await Task.CompletedTask;
    }

    private async Task HideWindow(MedicationTimeAndDetailDto medication)
    {
        if (medication?.SelectedMedication != null)
        {
            medication.SelectedMedication.IsSelected = false;
            medication.SelectedMedication = new MedicationDetailHistoryDto { };
        }
        medication.PrevSelectedMedication = null;
        medication.SelectedRecord = null;
        await Task.CompletedTask;
    }

    private void OnSlide()
    {
        if (SliderDates != null && SliderDates.Count() > 0)
            SliderValueText = SliderDates[30 - SliderValue];
    }

    private void GoToday()
    {
        if (SliderDates != null && SliderDates.Count() > 0)
        {
            SliderValueText = SliderDates[SliderMaxValue - 1];
            SliderValue = SliderMaxValue;
        }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Id), out var param))
            Id = Convert.ToInt32(param.ToString());

        await InitCommand.Execute();
    }
}