namespace VisitTracker;

public static class BaseAssembler
{
    public static string GetServiceUserAddress(IList<ServiceUserAddress> addresses, DateTime date)
    {
        var serviceUserAddress = addresses.FirstOrDefault(x => x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date));
        return serviceUserAddress?.Address;
    }

    public static async Task<List<BookingCardDto>> BuildBookingsCard(IList<Booking> dbBookings)
    {
        var bookingDetails = await AppServices.Current.BookingDetailService.GetAllByIds(dbBookings.Select(x => x.Id), "BookingId");
        var serviceUsers = await AppServices.Current.ServiceUserService.GetAllByIds(dbBookings.Select(x => x.ServiceUserId));
        var careWorkers = await AppServices.Current.CareWorkerService.GetAllByIds(bookingDetails.Select(x => x.CareWorkerId));
        var visits = (await AppServices.Current.VisitService.GetAllByIds(bookingDetails.Select(x => x.Id), "BookingDetailId")).ToList();
        var visitCompletedCode = AppData.Current.Codes.FirstOrDefault(x => x.Type == ECodeType.VISIT_STATUS.ToString() && x.Name == ECodeName.COMPLETED.ToString());

        visits = visits.Where(y => y.VisitStatusId == visitCompletedCode.Id).ToList();
        if (visits != null && visits.Any())
            foreach (var groupedVisits in visits.GroupBy(x => x.BookingDetailId))
            {
                var version = 1;
                foreach (var visit in groupedVisits.OrderBy(x => x.CompletedOnTicks))
                {
                    if (AppData.Current.CurrentProfile.Type == EUserType.SERVICEUSER.ToString())
                    {
                        visit.DisplayName = "VR";
                    }
                    else
                    {
                        visit.DisplayName = "VR-V" + version;
                        version++;
                    }
                }
            }

        var bookings = new List<BookingCardDto>();
        foreach (var booking in dbBookings.OrderBy(x => x.StartTime))
        {
            var bds = bookingDetails.Where(x => x.BookingId == booking.Id).ToList();

            var bookingCardDto = new BookingCardDto();
            bookingCardDto.Booking = booking;
            bookingCardDto.ServiceUser = serviceUsers.FirstOrDefault(x => x.Id == booking.ServiceUserId);
            if (bookingCardDto.ServiceUser != null)
            {
                bookingCardDto.ServiceUser.Gender = AppData.Current.Codes?
                    .FirstOrDefault(x => x.Id == bookingCardDto.ServiceUser.GenderId)?
                    .Name;
            }
            bookingCardDto.Visits = visits.Where(x => bds.Select(y => y.Id).Contains(x.BookingDetailId))
                .OrderBy(x => x.CompletedOnTicks).ToList();

            var primary = bookingDetails.FirstOrDefault(y => y.BookingId == booking.Id && y.IsMaster);
            bookingCardDto.PrimaryCareWorker = careWorkers.FirstOrDefault(x => x.Id == primary.CareWorkerId);
            if (primary.EtaAvailable)
                bookingCardDto.PrimaryCareWorkerEta = new EtaMobileDto
                {
                    Eta = primary.Eta,
                    EtaOn = primary.EtaOn,
                    EtaStatusColor = primary.EtaStatusColor,
                    EtaStatusText = primary.EtaStatusText,
                    EtaAvailable = primary.EtaAvailable
                };

            var secondary = bookingDetails.FirstOrDefault(y => y.BookingId == booking.Id && !y.IsMaster);
            if (secondary != null)
            {
                bookingCardDto.SecondaryCareWorker = careWorkers.FirstOrDefault(x => x.Id == secondary.CareWorkerId);
                if (secondary.EtaAvailable)
                    bookingCardDto.SecondaryCareWorkerEta = new EtaMobileDto
                    {
                        Eta = secondary.Eta,
                        EtaOn = secondary.EtaOn,
                        EtaStatusColor = secondary.EtaStatusColor,
                        EtaStatusText = secondary.EtaStatusText,
                        EtaAvailable = secondary.EtaAvailable
                    };
            }

            bookings.Add(bookingCardDto);
        }

        return bookings;
    }

    public static async Task<List<BookingsDto>> BuildBookingsSuViewModels(IList<Booking> dbBookings)
    {
        var serviceUsers = await AppServices.Current.ServiceUserService.GetAll();
        var serviceUserAddresses = await AppServices.Current.ServiceUserAddressService.GetAll();
        var careWorkers = await AppServices.Current.CareWorkerService.GetAll();

        var bookings = new List<BookingsDto>();
        foreach (var booking in dbBookings.OrderBy(x => x.StartTime))
        {
            var serviceUser = serviceUsers.FirstOrDefault(y => y.Id == booking.ServiceUserId);
            var serviceUserAddress = serviceUserAddresses.FirstOrDefault(x => x.EffectiveFrom <= booking.StartTime
                                        && (x.EffectiveTo == null || x.EffectiveTo >= booking.EndTime));
            var bookingDto = new BookingsDto()
            {
                Id = booking.Id,
                BookingFromTime = booking.StartTime.ToString("HH:mm tt").ToUpper(),
                BookingFromToTime = booking.StartTime.ToString("HH:mm tt").ToUpper() + " - " + booking.EndTime.ToString("HH:mm tt").ToUpper(),
                ServiceUserName = serviceUser.Name,
                ServiceUserImageUrl = serviceUser.ImageUrl,
                ServiceUserAddress = serviceUserAddress?.Address,
                IsCompleted = booking.IsCompleted.HasValue && booking.IsCompleted.Value,
            };

            var bookingDetails = await AppServices.Current.BookingDetailService.GetAllForBooking(booking.Id);
            bookingDto.CareWorkers = new List<BookingCareWorkerDto>();
            foreach (var bd in bookingDetails.OrderByDescending(x => x.IsMaster))
            {
                var bCwDto = new BookingCareWorkerDto();
                var cw = careWorkers.FirstOrDefault(x => x.Id == bd.CareWorkerId);
                bCwDto.BookingDetailId = bd.Id;
                bCwDto.Name = cw.Name;
                bCwDto.ImageUrl = cw.ImageUrl;

                bCwDto.IsMaster = bd.IsMaster;
                bCwDto.Eta = bd.Eta;
                bCwDto.EtaOn = bd.EtaOn;
                bCwDto.EtaStatusColor = bd.EtaStatusColor;
                bCwDto.EtaStatusText = bd.EtaStatusText;
                bCwDto.EtaAvailable = bd.EtaAvailable;

                bookingDto.CareWorkers.Add(bCwDto);
            }

            bookings.Add(bookingDto);
        }

        return bookings;
    }

    public static async Task<List<BookingsDto>> BuildBookingsCwViewModels(IList<Booking> dbBookings, IList<BookingDetail> bookingDetails, IList<CareWorker> careWorkers)
    {
        var serviceUserAddresses = await AppServices.Current.ServiceUserAddressService.GetAllByIds(dbBookings.Select(x => x.ServiceUserId), "ServiceUserId");
        var bookings = new List<BookingsDto>();
        foreach (var booking in dbBookings)
        {
            var bookingDetail = bookingDetails.Where(y => y.BookingId == booking.Id);
            var careWorker = careWorkers.FirstOrDefault(y => bookingDetail.Select(i => i.CareWorkerId).Contains(y.Id));
            bookings.Add(new BookingsDto()
            {
                Id = booking.Id,
                BookingFromTime = booking.StartTime.ToString("HH:mm tt").ToUpper(),
                BookingFromToTime = booking.StartTime.ToString("HH:mm tt").ToUpper() + " - " + booking.EndTime.ToString("HH:mm tt").ToUpper(),
                CareWorkerName = careWorker.Name,
                CareWorkerImageUrl = careWorker.ImageUrl,
                CareWorkerAddress = GetServiceUserAddress(serviceUserAddresses, booking.StartTime),
                IsCompleted = booking.IsCompleted.HasValue && booking.IsCompleted.Value
            });
        }
        return bookings;
    }

    public static async Task<ServiceUserDto> BuildSuViewModel(ServiceUser data, Booking booking)
    {
        if (data != null)
        {
            var serviceUserAddress = await AppServices.Current.ServiceUserAddressService.GetActiveAddressByDate(booking.ServiceUserId, booking.StartTime);
            var suGender = AppData.Current.Codes?.FirstOrDefault(x => x.Id == data.GenderId)?.Name;
            return new ServiceUserDto()
            {
                Id = data.Id,
                Address = serviceUserAddress?.Address,
                ImageUrl = data.ImageUrl,
                Name = data.Name,
                Phone = data.Phone,
                Latitude = serviceUserAddress.Latitude,
                Longitude = serviceUserAddress.Longitude,
                Gender = suGender,
            };
        }
        return null;
    }

    public static async Task<List<TaskDto>> BuildTaskViewModels(IList<BookingTask> Tasks, int visitId)
    {
        var tasks = new List<TaskDto>();
        if (Tasks != null)
        {
            var visitTasks = await AppServices.Current.VisitTaskService.GetAllByVisitId(visitId);
            foreach (var task in Tasks)
            {
                var visitTask = visitTasks.FirstOrDefault(x => x.TaskId == task.Id);
                tasks.Add(new TaskDto()
                {
                    Id = task.Id,
                    CompletedOn = visitTask?.CompletedOn,
                    IsVisited = visitTask != null,
                    Title = task.Title,
                    CompletionStatusColor = (visitTask?.CompletionStatusId != null)
                        ? Color.FromArgb(colorAsHex: "#00CD00"): Color.FromRgb(158, 53, 53),
                    Order = task.Order
                });
            }
        }
        return tasks;
    }

    public static async Task<List<MedicationDto>> BuildMedicationsViewModels(IList<BookingMedication> Medications, int visitId)
    {
        var MedicationsList = new List<MedicationDto>();
        if (Medications != null)
        {
            var medicationTasks = await AppServices.Current.VisitMedicationService.GetAllByVisitId(visitId);
            foreach (var medication in Medications)
            {
                var visitMedication = medicationTasks.FirstOrDefault(x => x.MedicationId == medication.Id);
                MedicationsList.Add(new MedicationDto()
                {
                    Id = medication.Id,
                    CompletedOn = visitMedication?.CompletedOn,
                    IsDrafted = visitMedication?.IsSaved ?? false,
                    Name = medication.Title,
                    CompletionStatusColor = (visitMedication?.CompletionStatusId != null)
                        ? Color.FromArgb(colorAsHex: "#00CD00"): Color.FromRgb(158, 53, 53),
                    AdminsterWarning = false,
                    Order = medication.Order
                });
            }
        }
        return MedicationsList;
    }

    public static FluidDto BuildFluidViewModels(VisitFluid visitFluid)
    {
        var fluid = new FluidDto();
        fluid.Id = visitFluid?.Id;
        fluid.CompletedOn = visitFluid?.CompletedOn;

        var fluidCompletionId = AppData.Current.Codes
            .FirstOrDefault(x => x.Type == ECodeType.FLUID_COMPLETION.ToString() && x.Name == ECodeName.COMPLETED.ToString())?.Id;
        fluid.CompletionStatusColor = (visitFluid?.CompletedOn != null && fluidCompletionId != null) ?
                Color.FromArgb(colorAsHex: "#00CD00") : Color.FromRgb(158, 53, 53);
        return fluid;
    }

    public static List<IncidentsDto> BuildIncidentViewModels(IList<Incident> data)
    {
        var Incidents = new List<IncidentsDto>();
        if (data != null)
        {
            foreach (var Incident in data)
            {
                Incidents.Add(new IncidentsDto()
                {
                    Id = Incident.Id,
                    CompletedOn = Incident.CompletedOn,
                    Summary = Incident.Summary
                });
            }
        }
        return Incidents;
    }

    public static IList<AttachmentDto> BuildAttachmentViewModels(IList<Attachment> data)
    {
        return data?.GroupBy(a => a.Type)
                .SelectMany(group => group
                .Select((a, index) => new AttachmentDto()
                {
                    Id = a.Id,
                    ServerRef = a.ServerRef,
                    FileName = a.FileName,
                    FilePath = Path.Combine(FileSystem.AppDataDirectory, a.FileName),
                    AttachmentType = (EAttachmentType)Enum.Parse(typeof(EAttachmentType), a.Type),
                    DisplayIcon = a.Type == EAttachmentType.Image.ToString() ? MaterialCommunityIconsFont.Eye : MaterialCommunityIconsFont.PlayCircle
                }))
                .ToList() ?? new List<AttachmentDto>();
    }

    public static ObservableCollection<string> BuildVisitMessageStringViewModels(IEnumerable<VisitMessage> data)
    {
        if (data == null || !data.Any())
            return null;

        return data.Select(x => x.Message).ToObservableCollection();
    }

    public static string BuildVisitConsumableStringViewModels(IList<VisitConsumable> data)
    {
        if (data == null || !data.Any())
            return null;

        return string.Join("<br/>", data.Select(y => "&#x2022;&nbsp; &nbsp;"
            + AppData.Current.Codes?.Where(i => i.Id == y.ConsumableTypeId)?.FirstOrDefault()?.Name +
            " (" + y.QuantityUsed + ")"));
    }

    public static string BuildVisitMessageStringViewModels(IList<VisitMessage> data, IList<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return null;

        return string.Join("<br/>", data.Where(x => selectedIds.Contains(x.Id)).Select(y => "&#x2022;&nbsp; &nbsp;" + y.Message));
    }

    public static Attachment BuildAttachment(AttachmentDto attachment, int visitId, string type, int typeId)
    {
        if (attachment == null || string.IsNullOrEmpty(type) || typeId == default) return null;
        Attachment dbAttachment = new Attachment() { BaseVisitId = visitId == 0 ? null : visitId };

        if (type == EPageType.BookingType.ToString()) dbAttachment.VisitId = typeId;
        else if (type == EPageType.TaskType.ToString()) dbAttachment.VisitTaskId = typeId;
        else if (type == EPageType.MedicationType.ToString()) dbAttachment.VisitMedicationId = typeId;
        else if (type == EPageType.FluidType.ToString()) dbAttachment.VisitFluidId = typeId;
        else if (type == EPageType.IncidentType.ToString()) dbAttachment.IncidentId = typeId;
        else if (type == EPageType.BodyMapType.ToString()) dbAttachment.BodyMapId = typeId;

        dbAttachment.FileName = attachment.FileName;
        dbAttachment.Type = attachment.AttachmentType.ToString();
        dbAttachment.AddedOn = DateTimeExtensions.NowNoTimezone();
        return dbAttachment;
    }

    public static (BodyMapCreateDto, BodyMapNotesPopupVm) BuildBodyMapPopupViewModels(BodyMap bodyMap, IList<Attachment> attachments)
    {
        var isRear = bodyMap?.Parts?.Split(',')?.Any(x => Enumerable.Range(35, 70).Contains(Convert.ToInt32(x)));
        BodyMapCreateDto bodyMapCreateDto = new BodyMapCreateDto
        {
            BodyMapLabel = Constants.BodyMapBack,
            NotesEnabled = !string.IsNullOrEmpty(bodyMap?.Notes),
        };
        var BodyMapNotesPopupVm = new BodyMapNotesPopupVm()
        {
            Id = bodyMap.Id,
            BaseVisitId = bodyMap.BaseVisitId,
            Summary = bodyMap?.Notes,
            Parts = bodyMap?.Parts,
            popupWidth = SystemHelper.Current.SetCardViewWidth(10, true, 500),
            AttachmentList = BuildAttachmentViewModels(attachments)
        };
        return (bodyMapCreateDto, BodyMapNotesPopupVm);
    }

    public static (TotalBookingCountsDto, TotalBookingCountsDto) BuildTotalBookingsViewModel(IList<Booking> totalBookings)
    {
        var TotalBookingCounts = new TotalBookingCountsDto();
        var CompletedBookingCounts = new TotalBookingCountsDto();

        var today = DateTimeExtensions.NowNoTimezone();
        var days = GetCurrentWeekDates();
        foreach (var day in days)
            switch (day.Date.ToString("ddd")?.ToUpper())
            {
                case "MON":
                    var mon = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    TotalBookingCounts.Mon = mon?.Count() ?? 0;
                    CompletedBookingCounts.Mon = mon?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    CompletedBookingCounts.MonColor = TotalBookingCounts.MonColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "TUE":
                    var tue = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    TotalBookingCounts.Tue = tue?.Count() ?? 0;
                    CompletedBookingCounts.Tue = tue?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    CompletedBookingCounts.TueColor = TotalBookingCounts.TueColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "WED":
                    var wed = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    CompletedBookingCounts.Wed = wed?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    TotalBookingCounts.Wed = wed?.Count() ?? 0;
                    CompletedBookingCounts.WedColor = TotalBookingCounts.WedColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "THU":
                    var thu = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    CompletedBookingCounts.Thu = thu?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    TotalBookingCounts.Thu = thu?.Count() ?? 0;
                    CompletedBookingCounts.ThuColor = TotalBookingCounts.ThuColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "FRI":
                    var fri = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    CompletedBookingCounts.Fri = fri?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    TotalBookingCounts.Fri = fri?.Count() ?? 0;
                    CompletedBookingCounts.FriColor = TotalBookingCounts.FriColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "SAT":
                    var sat = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    CompletedBookingCounts.Sat = sat?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    TotalBookingCounts.Sat = sat?.Count() ?? 0;
                    CompletedBookingCounts.SatColor = TotalBookingCounts.SatColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "SUN":
                    var sun = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    CompletedBookingCounts.Sun = sun?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    TotalBookingCounts.Sun = sun?.Count() ?? 0;
                    CompletedBookingCounts.SunColor = TotalBookingCounts.SunColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;
            }

        return (CompletedBookingCounts, TotalBookingCounts);
    }

    public static TotalBookingCountsDto BuildCompletedBookingsCountViewModel(IList<Booking> totalBookings)
    {
        var completedBookings = new TotalBookingCountsDto();

        var today = DateTimeExtensions.NowNoTimezone();
        var days = GetCurrentWeekDates();
        foreach (var day in days)
            switch (day.Date.ToString("ddd")?.ToUpper())
            {
                case "MON":
                    var mon = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Mon = mon?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.MonColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "TUE":
                    var tue = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Tue = tue?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.TueColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "WED":
                    var wed = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Wed = wed?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.WedColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "THU":
                    var thu = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Thu = thu?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.ThuColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "FRI":
                    var fri = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Fri = fri?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.FriColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "SAT":
                    var sat = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Sat = sat?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.SatColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;

                case "SUN":
                    var sun = totalBookings.Where(i => i.StartTime.Date == day.Date)?.ToList();
                    completedBookings.Sun = sun?.Count(i => i.IsCompleted.HasValue && i.IsCompleted.Value) ?? 0;
                    completedBookings.SunColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                    break;
            }

        return completedBookings;
    }

    public static TotalBookingCountsDto BuildTotalDayBookingsViewModel(IList<Booking> totalBookings)
    {
        var bookings = new TotalBookingCountsDto();

        if (totalBookings != null && totalBookings.Any())
        {
            var today = DateTimeExtensions.NowNoTimezone();
            var days = GetCurrentWeekDates();
            foreach (var day in days)
            {
                switch (day.Date.ToString("ddd")?.ToUpper())
                {
                    case "MON":
                        var mon = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Mon = mon?.Count() ?? 0;
                        bookings.MonColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;

                    case "TUE":
                        var tue = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Tue = tue?.Count() ?? 0;
                        bookings.TueColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;

                    case "WED":
                        var wed = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Wed = wed?.Count() ?? 0;
                        bookings.WedColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;

                    case "THU":
                        var thu = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Thu = thu?.Count() ?? 0;
                        bookings.ThuColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;

                    case "FRI":
                        var fri = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Fri = fri?.Count() ?? 0;
                        bookings.FriColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;

                    case "SAT":
                        var sat = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Sat = sat?.Count() ?? 0;
                        bookings.SatColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;

                    case "SUN":
                        var sun = totalBookings.Where(i => i.StartTime.Date == day.Date).ToList();
                        bookings.Sun = sun?.Count() ?? 0;
                        bookings.SunColor = Application.Current.Resources[day.Date.ToString("ddd")?.ToUpper() == today.ToString("ddd")?.ToUpper() ? "PrimaryColor" : "TextColor"] as Color;
                        break;
                }
            }
        }

        return bookings;
    }

    public static List<DateTime> GetCurrentWeekDates()
    {
        var today = DateTimeExtensions.NowNoTimezone().Date;
        int currentDayOfWeek = (int)today.DayOfWeek;
        int daysToMonday = (currentDayOfWeek == 0) ? -6 : (1 - currentDayOfWeek);
        DateTime monday = today.AddDays(daysToMonday);

        var weekDates = new List<DateTime>();
        for (int i = 0; i < 7; i++) weekDates.Add(monday.AddDays(i));

        return weekDates;
    }

    public static FluidChartDto BuildFluidChartViewModels(FluidChartResponse fluidChart)
    {
        if (fluidChart != null)
            return new FluidChartDto()
            {
                IvScIntakeTotal = fluidChart.IvScIntakeTotal,
                OralIntakeTotal = fluidChart.OralIntakeTotal,
                OtherIntakeTotal = fluidChart.OtherIntakeTotal,
                OtherOutputTotal = fluidChart.OtherOutputTotal,
                TodayBalance = fluidChart.TodayBalance,
                TodayBalanceTime = fluidChart.TodayBalanceTime + " hrs",
                TubeOutputTotal = fluidChart.TubeOutputTotal,
                UrineOutputTotal = fluidChart.UrineOutputTotal,
                VomitOutputTotal = fluidChart.VomitOutputTotal,
                YesterdayBalance = fluidChart.YesterdayBalance,
                FluidsList = BuildFluidHistoryList(fluidChart)
            };
        return null;
    }

    private static List<FluidHistoryDto> BuildFluidHistoryList(FluidChartResponse fluidChart)
    {
        var fluidHistory = new List<FluidHistoryDto>();
        if (fluidChart?.Fluids != null)
            foreach (var item in fluidChart?.Fluids)
            {
                if (item.CompletedOn != null)
                {
                    fluidHistory.Add(new FluidHistoryDto()
                    {
                        OralIntake = item.OralIntake,
                        IvScIntake = item.IvScIntake,
                        OtherIntake = item.OtherIntake,
                        UrineOutput = item.UrineOutput,
                        VomitOutput = item.VomitOutput,
                        TubeOutput = item.TubeOutput,
                        OtherOutput = item.OtherOutput,
                        Hour = item.Hour
                    });
                }
            }
        return fluidHistory;
    }

    public static List<MedicationsDto> BuildMedicationsViewModel(List<MedicationEntry> Medications)
    {
        if (Medications == null)
            return null;

        var medications = new List<MedicationsDto>();
        foreach (var item in Medications)
        {
            medications.Add(new MedicationsDto()
            {
                Dosage = item.Dosage,
                GracePriod = item.GracePriod,
                MealInstructions = item.MealInstructions,
                MedicationName = item.MedicationName,
                Mode = item.AdministrationMode,
                Route = item.Route,
                Strength = item.Strength,
                MedicationTimeAndDetailList = BuildMedicationTimeAndDetailViewModel(item.MedicationTimeSlots).ToObservableCollection()
            });
        }

        return medications;
    }

    public static List<MedicationEntry> BuildMarChart(IList<BookingMedication> medicationHistory,
        IList<VisitMedicationDto> medicationVisits)
    {
        DateTime endDate = DateTimeExtensions.NowNoTimezone();
        DateTime startDate = DateTimeExtensions.NowNoTimezone().AddDays(-Constants.NoOfDaysMarChart);

        var response = new List<MedicationEntry>();
        var daysInRange = Enumerable.Range(0, Constants.NoOfDaysMarChart)
                                    .Select(offset => endDate.AddDays(-offset))
                                    .OrderBy(x => x)
                                    .ToList();

        var medicationHistoryGrouped = medicationHistory?.Where(x => x.ExternalMedicationSlotRef != 0).GroupBy(m => m.ExternalMedicationRef);
        if (medicationHistoryGrouped != null && medicationHistoryGrouped.Any())
        {
            foreach (var medicationGroup in medicationHistoryGrouped)
            {
                var medicationEntry = new MedicationEntry
                {
                    MedicationName = medicationGroup.First().Title,
                    Dosage = medicationGroup.First().DosageUnitStr,
                    AdministrationMode = medicationGroup.First()?.AdministrationModeStr ?? string.Empty,
                    MealInstructions = medicationGroup.First()?.MealInstructionStr ?? string.Empty,
                    Route = medicationGroup.First()?.RouteStr ?? string.Empty,
                    Strength = medicationGroup.First().Strength,
                    MedicationTimeSlots = new List<MedicationTimeSlot>()
                };

                var medicationTimeSlotsGrouped = medicationGroup.GroupBy(m => m.ExternalMedicationSlotRef).OrderBy(j => j.Key);
                foreach (var timeSlotGroup in medicationTimeSlotsGrouped)
                {
                    var firstSlot = timeSlotGroup.First();
                    var medicationTimeSlot = new MedicationTimeSlot
                    {
                        Time = $"{string.Format("{0:D2}:{1:D2}", firstSlot.FromHour, firstSlot.FromMinute)} " +
                                $"- {string.Format("{0:D2}:{1:D2}", firstSlot.ToHour, firstSlot.ToMinute)}",
                        MedicationDetails = new List<MedicationDetailEntry>()
                    };
                    var daysRecord = new Dictionary<string, MedicationDetailEntry>();

                    foreach (var day in daysInRange)
                    {
                        daysRecord[day.ToString("MMM") + day.ToString("dd")] = new MedicationDetailEntry
                        {
                            Month = day.ToString("MMM"),
                            Date = day.ToString("dd"),
                            Day = day.ToString("ddd")
                        };
                    }

                    foreach (var medication in timeSlotGroup)
                    {
                        if (daysRecord.ContainsKey(medication.StartDateTime.ToString("MMM") + medication.StartDateTime.ToString("dd")))
                        {
                            var medicationVisit = medicationVisits?.FirstOrDefault(x => x.MedicationId == medication.Id);
                            if (medicationVisit == null) continue;
                            var medEntry = daysRecord[medication.StartDateTime.ToString("MMM") + medication.StartDateTime.ToString("dd")];
                            medEntry.Id = medicationVisit.Id;
                            medEntry.BackgroundColor = medicationVisit.BackgroundColor;
                        }
                    }
                    medicationTimeSlot.MedicationDetails = daysRecord.Select(i => i.Value).ToList();
                    medicationEntry.MedicationTimeSlots.Add(medicationTimeSlot);
                }

                response.Add(medicationEntry);
            }
        }

        return response;
    }

    private static List<MedicationTimeAndDetailDto> BuildMedicationTimeAndDetailViewModel(List<MedicationTimeSlot> medicationtTimeAndDetails)
    {
        var data = new List<MedicationTimeAndDetailDto>();
        foreach (var item in medicationtTimeAndDetails)
        {
            data.Add(new MedicationTimeAndDetailDto()
            {
                Time = item.Time,
                MedicationDetails = BuildMedicationDetailsViewModel(item.MedicationDetails).ToObservableCollection()
            });
        }
        return data;
    }

    private static List<MedicationDetailHistoryDto> BuildMedicationDetailsViewModel(List<MedicationDetailEntry> medicationDetailList)
    {
        var data = new List<MedicationDetailHistoryDto>();
        foreach (var meddetail in medicationDetailList)
        {
            data.Add(new MedicationDetailHistoryDto()
            {
                Id = meddetail.Id,
                Month = meddetail.Month,
                Date = meddetail.Date,
                Day = meddetail.Day,
                BackgroundColor = Color.FromArgb(meddetail.BackgroundColor),
            });
        }
        return data;
    }

    public static async Task<OngoingDto> BuildOngoingViewModels(Booking booking, Visit Visit)
    {
        var now = DateTimeExtensions.NowNoTimezone();

        var ongoingDto = new OngoingDto();
        var serviceUser = await AppServices.Current.ServiceUserService.GetById(booking.ServiceUserId);
        var bookingDetail = Visit == null ?
            await AppServices.Current.BookingDetailService.GetBookingDetailForCurrentCw(booking.Id)
            : await AppServices.Current.BookingDetailService.GetById(Visit.BookingDetailId);
        var bookingDetails = await AppServices.Current.BookingDetailService.GetAllForBooking(booking.Id);
        var careWorkers = await AppServices.Current.CareWorkerService.GetAllByIds(bookingDetails.Select(x => x.CareWorkerId));
        var tasks = await AppServices.Current.TaskService.GetAllByBookingId(booking.Id);
        var medications = await AppServices.Current.MedicationService.GetAllByBookingId(booking.Id);
        var codeConsumables = await AppServices.Current.CodeService.GetAllByType(ECodeType.CONSUMABLE_TYPE);

        IList<VisitConsumable> consumables = null;
        if (Visit != null)
        {
            if (string.IsNullOrEmpty(Visit.MachineInfo) || string.IsNullOrEmpty(Visit.DeviceInfo))
            {
                Visit.MachineInfo = AppServices.Current.AppPreference.DeviceInfo;
                Visit.DeviceInfo = DeviceInfo.Platform.ToString();
                Visit = await AppServices.Current.VisitService.InsertOrReplace(Visit);
            }

            var fluid = await AppServices.Current.VisitFluidService.GetByVisitId(Visit.Id);
            var bodyMaps = await AppServices.Current.BodyMapService.GetAllByVisitId(Visit.Id);
            var incidents = await AppServices.Current.IncidentService.GetAllByVisitId(Visit.Id);
            var attachments = await AppServices.Current.AttachmentService.GetAllByVisitId(Visit.Id);
            consumables = await AppServices.Current.VisitConsumableService.GetAllByVisitId(Visit.Id);

            ongoingDto.Fluid = BuildFluidViewModels(fluid);
            ongoingDto.BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
            ongoingDto.IncidentList = incidents.OrderBy(i => i.CompletedOn)
                                .Select((incident, index) =>
                                {
                                    incident.DisplayName = $"IR-{index + 1}";
                                    return incident;
                                }).ToList();
            ongoingDto.AttachmentList = BuildAttachmentViewModels(attachments);
        }

        ongoingDto.ConsumableList = new List<VisitConsumableDto>();
        foreach (var codeConsumable in codeConsumables)
        {
            var consumable = consumables?.FirstOrDefault(x => x.ConsumableTypeId == codeConsumable.Id);
            var visitConsumable = new VisitConsumableDto
            {
                ConsumableTypeId = codeConsumable.Id,
                QuantityUsed = consumable?.QuantityUsed ?? default,
                ConsumableTypeStr = codeConsumable.Name,
            };
            ongoingDto.ConsumableList.Add(visitConsumable);
        }

        if (careWorkers.Count == 1)
        {
            ongoingDto.IsMaster = true;
            careWorkers.ToList().ForEach(x => x.IsMaster = true);
        }
        else
        {
            bookingDetails.ToList().ForEach(y =>
            {
                var cw = careWorkers.FirstOrDefault(x => x.Id == y.CareWorkerId);
                cw.IsMaster = y.IsMaster;
                if (cw.IsMaster && cw.Id == AppData.Current.CurrentProfile.Id)
                    ongoingDto.IsMaster = true;
            });
        }

        ongoingDto.CareWorkers = new List<BookingCareWorkerDto>();
        foreach (var bd in bookingDetails.OrderByDescending(x => x.IsMaster))
        {
            var bCwDto = new BookingCareWorkerDto();
            var cw = careWorkers.FirstOrDefault(x => x.Id == bd.CareWorkerId);
            bCwDto.BookingDetailId = bd.Id;
            bCwDto.Name = cw.Name;
            bCwDto.ImageUrl = cw.ImageUrl;

            bCwDto.IsMaster = bd.IsMaster;
            bCwDto.Eta = bd.Eta;
            bCwDto.EtaOn = bd.EtaOn;
            bCwDto.EtaStatusColor = bd.EtaStatusColor;
            bCwDto.EtaStatusText = bd.EtaStatusText;
            bCwDto.EtaAvailable = bd.EtaAvailable;

            ongoingDto.CareWorkers.Add(bCwDto);
        }

        ongoingDto.Booking = booking;
        ongoingDto.Visit = Visit;
        ongoingDto.BookingDetail = bookingDetail;
        ongoingDto.BookingDetails = bookingDetails;
        ongoingDto.ServiceUser = serviceUser;
        ongoingDto.ServiceUserCard = await BuildUserCardFromSu(serviceUser, booking);
        ongoingDto.BookingFromToTime = booking.StartTime.ToString("HH:mm tt") + " - " + booking.EndTime.ToString("HH:mm tt") + Environment.NewLine + booking.StartTime.ToString("ddd, dd MMM yyyy");
        ongoingDto.Notes = serviceUser.Notes;
        ongoingDto.HandOverNotes = booking.HandOverNotes;
        ongoingDto.Summary = Visit?.Summary;
        ongoingDto.IsFluidApplicable = booking.FluidChartsApplicable;
        ongoingDto.BookingTypeWithCode = booking.BookingTypeWithCode;
        ongoingDto.BookingStatus = booking.CompletionStatusId.HasValue
                ? AppData.Current.Codes?.Where(i => i.Id == booking.CompletionStatusId)?.FirstOrDefault()?.Name
                : AppData.Current.Codes?.Where(i => i.Id == booking.BookingStatusId)?.FirstOrDefault()?.Name;
        ongoingDto.TaskList = await BuildTaskViewModels(tasks, Visit?.Id ?? default);
        ongoingDto.MedicationList = await BuildMedicationsViewModels(medications, Visit?.Id ?? default);
        ongoingDto.ShortRemarks = BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(x => x.Type == EMessageType.SHORT_REMARKS.ToString()).ToList());
        ongoingDto.HealthStatuses = BuildVisitMessageStringViewModels(AppData.Current.VisitMessages?.Where(x => x.Type == EMessageType.HEALTH_STATUS.ToString()).ToList());
        return ongoingDto;
    }

    public static async Task<BookingDetailDto> BuildBookingDetailViewModels(Booking booking, Visit Visit)
    {
        var bookingDetailDto = new BookingDetailDto();
        var serviceUser = await AppServices.Current.ServiceUserService.GetById(booking.ServiceUserId);
        var bookingDetails = await AppServices.Current.BookingDetailService.GetAllForBooking(booking.Id);
        var careWorkers = await AppServices.Current.CareWorkerService.GetAllByIds(bookingDetails.Select(x => x.CareWorkerId));
        var tasks = await AppServices.Current.TaskService.GetAllByBookingId(booking.Id);
        var medications = await AppServices.Current.MedicationService.GetAllByBookingId(booking.Id);
        var codeConsumables = await AppServices.Current.CodeService.GetAllByType(ECodeType.CONSUMABLE_TYPE);

        bookingDetailDto.Visit = Visit ?? await AppServices.Current.VisitService.GetByBookingId(booking.Id);

        if (bookingDetailDto.Visit != null)
        {
            var fluid = await AppServices.Current.VisitFluidService.GetByVisitId(bookingDetailDto.Visit.Id);
            var bodyMaps = await AppServices.Current.BodyMapService.GetAllByVisitId(bookingDetailDto.Visit.Id);
            var incidents = await AppServices.Current.IncidentService.GetAllByVisitId(bookingDetailDto.Visit.Id);
            var attachments = await AppServices.Current.AttachmentService.GetAllByVisitId(bookingDetailDto.Visit.Id);
            var consumables = await AppServices.Current.VisitConsumableService.GetAllByVisitId(bookingDetailDto.Visit.Id);
            var shortRemarks = await AppServices.Current.VisitShortRemarkService.GetAllByVisitId(bookingDetailDto.Visit.Id);
            var healthStatuses = await AppServices.Current.VisitHealthStatusService.GetAllByVisitId(bookingDetailDto.Visit.Id);

            bookingDetailDto.Summary = bookingDetailDto.Visit.Summary;
            bookingDetailDto.Fluid = BuildFluidViewModels(fluid);
            bookingDetailDto.BodyMapList = bodyMaps.OrderBy(i => i.AddedOn).ToList();
            bookingDetailDto.IncidentList = incidents.OrderBy(i => i.CompletedOn)
                                .Select((incident, index) =>
                                {
                                    incident.DisplayName = $"IR-{index + 1}";
                                    return incident;
                                }).ToList();
            bookingDetailDto.AttachmentList = BuildAttachmentViewModels(attachments);
            bookingDetailDto.SelectedConsumables = BuildVisitConsumableStringViewModels(consumables?.ToList());
            bookingDetailDto.SelectedHealthStatus = BuildVisitMessageStringViewModels(AppData.Current.VisitMessages, healthStatuses.Select(x => x.HealthStatusId).ToList());
            bookingDetailDto.SelectedShortRemark = BuildVisitMessageStringViewModels(AppData.Current.VisitMessages, shortRemarks.Select(x => x.ShortRemarkId).ToList());
            bookingDetailDto.IsVisited = bookingDetailDto.Visit != null;
            bookingDetailDto.Fluid.IsVisited = bookingDetailDto.Visit != null;
        }
        else
        {
            bookingDetailDto.Fluid = BuildFluidViewModels(null);
            bookingDetailDto.Fluid.IsVisited = false;
        }

        bookingDetailDto.Id = booking.Id;
        bookingDetailDto.Notes = serviceUser.Notes;
        bookingDetailDto.HandOverNotes = booking.HandOverNotes;
        bookingDetailDto.BookingStatus = booking.CompletionStatusId.HasValue
            ? AppData.Current.Codes?.Where(i => i.Id == booking.CompletionStatusId)?.FirstOrDefault()?.Name
            : AppData.Current.Codes?.Where(i => i.Id == booking.BookingStatusId)?.FirstOrDefault()?.Name;

        bookingDetailDto.BookingStatusColor = booking.CompletionStatusId.HasValue
            ? AppData.Current.Codes?.Where(i => i.Id == booking.CompletionStatusId)?.FirstOrDefault()?.Color
            : AppData.Current.Codes?.Where(i => i.Id == booking.BookingStatusId)?.FirstOrDefault()?.Color;

        bookingDetailDto.ServiceUser = await BuildSuViewModel(serviceUser, booking);
        bookingDetailDto.ServiceUserCard = await BuildUserCardFromSu(serviceUser, booking);
        bookingDetailDto.TaskList = await BuildTaskViewModels(tasks, bookingDetailDto?.Visit?.Id ?? default);
        bookingDetailDto.MedicationList = await BuildMedicationsViewModels(medications, bookingDetailDto?.Visit?.Id ?? default);
        bookingDetailDto.IsFluidApplicable = booking.FluidChartsApplicable;
        bookingDetailDto.PageTitle = booking.IsCompleted.HasValue && booking.IsCompleted.Value ? "Report" : "Booking";
        return bookingDetailDto;
    }

    public static async Task<UserCardDto> BuildUserCardFromSu(ServiceUser serviceUser,
        Booking booking, ServiceUserAddress serviceUserAddress = null)
    {
        string transportType = null;
        if (AppData.Current.CurrentProfile?.Type == nameof(EUserType.CAREWORKER))
        {
            var loggedInCw = await AppServices.Current.CareWorkerService.GetById(AppData.Current.CurrentProfile.Id);
            transportType = loggedInCw?.TransportTypeId != null ?
                AppData.Current.Codes?.FirstOrDefault(x => x.Id == loggedInCw?.TransportTypeId)?.Name
                : null;
        }

        if (serviceUserAddress == null)
            serviceUserAddress = booking != null
                ? await AppServices.Current.ServiceUserAddressService.GetActiveAddressByDate(booking.ServiceUserId, booking.StartTime)
                : await AppServices.Current.ServiceUserAddressService.GetActiveAddressNow(serviceUser.Id);

        var suGender = AppData.Current.Codes?.FirstOrDefault(x => x.Id == serviceUser.GenderId)?.Name;

        var userCard = new UserCardDto
        {
            UserId = serviceUser.Id,
            UserType = EUserType.SERVICEUSER,
            Name = serviceUser.Name,
            Phone = serviceUser.Phone,
            ImageUrl = serviceUser.ImageUrl,
            Gender = suGender,
            TransportationMode = transportType,
        };

        if (booking != null)
            userCard.UserBookingCard = new UserBookingCardDto
            {
                BookingFromToTime = booking.BookingFromToTime,
                BookingTypeWithCode = booking.BookingTypeWithCode,
            };

        if (serviceUserAddress != null)
            userCard.UserAddressCard = new UserAddressCardDto
            {
                Address = serviceUserAddress.Address,
                Latitude = serviceUserAddress.Latitude,
                Longitude = serviceUserAddress.Longitude,
                KeySafePIN = serviceUserAddress.KeySafePIN,
                EntryInstructions = serviceUserAddress.EntryInstructions
            };
        if (!string.IsNullOrEmpty(serviceUserAddress?.HousePhotos))
        {
            var housePhotos = JsonExtensions.Deserialize<List<string>>(serviceUserAddress?.HousePhotos);
            if (housePhotos != null && housePhotos.Any(x => !string.IsNullOrEmpty(x)))
                userCard.UserAddressCard.HousePhotos = housePhotos.Select(x => new AttachmentDto { S3Url = x }).ToList();
        }

        return userCard;
    }

    public static async Task<IList<UserCardDto>> BuildUserCardsFromSu(IList<ServiceUser> serviceUsers)
    {
        var serviceUserIds = serviceUsers.Select(x => x.Id).ToList();
        var allServiceUserAddresses = await AppServices.Current.ServiceUserAddressService.GetAll();

        var userCards = new List<UserCardDto>();
        foreach (var serviceUser in serviceUsers)
        {
            var date = DateTimeExtensions.NowNoTimezone();
            var serviceUserAddress = allServiceUserAddresses.FirstOrDefault(x => x.ServiceUserId == serviceUser.Id
                && x.EffectiveFromTicks <= date.Ticks && (x.EffectiveToTicks == default || x.EffectiveToTicks >= date.Ticks));
            var suGender = AppData.Current.Codes?.FirstOrDefault(x => x.Id == serviceUser.GenderId)?.Name;

            var userCard = new UserCardDto
            {
                UserId = serviceUser.Id,
                UserType = EUserType.SERVICEUSER,
                Name = serviceUser.Name,
                Phone = serviceUser.Phone,
                ImageUrl = serviceUser.ImageUrl,
                Gender = suGender,
            };

            if (serviceUserAddress != null)
                userCard.UserAddressCard = new UserAddressCardDto
                {
                    Address = serviceUserAddress.Address,
                    Latitude = serviceUserAddress.Latitude,
                    Longitude = serviceUserAddress.Longitude,
                    KeySafePIN = serviceUserAddress.KeySafePIN,
                    EntryInstructions = serviceUserAddress.EntryInstructions
                };
            if (!string.IsNullOrEmpty(serviceUserAddress?.HousePhotos))
            {
                var housePhotos = JsonExtensions.Deserialize<List<string>>(serviceUserAddress?.HousePhotos);
                if (housePhotos != null && housePhotos.Any(x => !string.IsNullOrEmpty(x)))
                    userCard.UserAddressCard.HousePhotos = housePhotos.Select(x => new AttachmentDto { S3Url = x }).ToList();
            }

            userCards.Add(userCard);
        }

        return userCards;
    }

    public static async Task<UserCardDto> BuildUserCardFromCw(CareWorker careWorker,
        Booking booking, CareWorkerAddress careWorkerAddress)
    {
        var loggedInCw = await AppServices.Current.CareWorkerService.GetById(AppData.Current.CurrentProfile.Id);
        var transportType = await AppServices.Current.CodeService.GetById(loggedInCw?.TransportTypeId ?? 0);
        var cwGender = AppData.Current.Codes?.FirstOrDefault(x => x.Id == careWorker.GenderId)?.Name;

        var userCard = new UserCardDto
        {
            UserId = careWorker.Id,
            UserType = EUserType.CAREWORKER,
            Name = careWorker.Name,
            Phone = careWorker.PhoneNo,
            ImageUrl = careWorker.ImageUrl,
            Gender = cwGender,
            GenderId = careWorker.GenderId,
            TransportationMode = transportType?.Name,
        };

        if (booking != null)
            userCard.UserBookingCard = new UserBookingCardDto
            {
                BookingFromToTime = booking.BookingFromToTime,
                BookingTypeWithCode = booking.BookingTypeWithCode,
            };

        if (careWorkerAddress != null)
            userCard.UserAddressCard = new UserAddressCardDto
            {
                Address = careWorkerAddress.Address,
                Latitude = careWorkerAddress.Latitude,
                Longitude = careWorkerAddress.Longitude,
            };

        return userCard;
    }
}