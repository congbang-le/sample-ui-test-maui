namespace VisitTracker.Shared;

/// <summary>
/// This class contains various messaging events used in the application.
/// It defines a set of message classes that can be used to communicate between different parts of the application.
/// </summary>
public static class MessagingEvents
{
    public class NotificationMessage : ValueChangedMessage<int>
    {
        public NotificationMessage(int value) : base(value)
        {
        }
    }

    public class TabBadgeCountMessage : ValueChangedMessage<(int, int)>
    {
        //Tupe of (int, int) is used to pass the index of the tab and the count of the badge
        public TabBadgeCountMessage((int, int) value) : base(value)
        {
        }
    }

    public class QrProviderCodeMessage : ValueChangedMessage<string>
    {
        public QrProviderCodeMessage(string value) : base(value)
        {
        }
    }

    public class FingerprintStartedMessage : ValueChangedMessage<bool>
    {
        public FingerprintStartedMessage(bool value) : base(value)
        {
        }
    }

    public class FingerprintCompletedMessage : ValueChangedMessage<bool>
    {
        public FingerprintCompletedMessage(bool value) : base(value)
        {
        }
    }

    public class FingerprintProgressMessage : ValueChangedMessage<float>
    {
        public FingerprintProgressMessage(float value) : base(value)
        {
        }
    }

    public class CareWorkerVisitStartedMessage : ValueChangedMessage<(bool, int)>
    {
        public CareWorkerVisitStartedMessage((bool, int) value) : base(value)
        {
        }
    }

    public class SupervisorVisitStartedMessage : ValueChangedMessage<bool>
    {
        public SupervisorVisitStartedMessage(bool value) : base(value)
        {
        }
    }

    public class VisitCompletedMessage : ValueChangedMessage<bool>
    {
        public VisitCompletedMessage(bool value) : base(value)
        {
        }
    }

    public class ServiceUserDetailPageBookingEditCompleted : ValueChangedMessage<bool>
    {
        public ServiceUserDetailPageBookingEditCompleted(bool value) : base(value)
        {
        }
    }

    public class CareWorkerDetailPageBookingEditCompleted : ValueChangedMessage<bool>
    {
        public CareWorkerDetailPageBookingEditCompleted(bool value) : base(value)
        {
        }
    }

    public class PageTabTitleChangeMessage : ValueChangedMessage<string>
    {
        public PageTabTitleChangeMessage(string value) : base(value)
        {
        }
    }

    public class BookingPageIncidentReportChangedMessage : ValueChangedMessage<int>
    {
        public BookingPageIncidentReportChangedMessage(int value) : base(value)
        {
        }
    }

    public class BookingPageTaskChangedMessage : ValueChangedMessage<int>
    {
        public BookingPageTaskChangedMessage(int value) : base(value)
        {
        }
    }

    public class BookingPageMedicationChangedMessage : ValueChangedMessage<int>
    {
        public BookingPageMedicationChangedMessage(int value) : base(value)
        {
        }
    }

    public class BookingPageFluidChangedMessage : ValueChangedMessage<int>
    {
        public BookingPageFluidChangedMessage(int value) : base(value)
        {
        }
    }

    public class BookingPageBodyMapChangedMessage : ValueChangedMessage<int>
    {
        public BookingPageBodyMapChangedMessage(int value) : base(value)
        {
        }
    }

    public class TaskPageBodyMapChangedMessage : ValueChangedMessage<bool>
    {
        public TaskPageBodyMapChangedMessage(bool value) : base(value)
        {
        }
    }

    public class MedicationPageUpdateReceivedMessage : ValueChangedMessage<bool>
    {
        public MedicationPageUpdateReceivedMessage(bool value) : base(value)
        {
        }
    }

    public class MedicationPageBodyMapChangedMessage : ValueChangedMessage<bool>
    {
        public MedicationPageBodyMapChangedMessage(bool value) : base(value)
        {
        }
    }

    public class FluidPageBodyMapChangedMessage : ValueChangedMessage<bool>
    {
        public FluidPageBodyMapChangedMessage(bool value) : base(value)
        {
        }
    }

    public class IncidentPageBodyMapChangedMessage : ValueChangedMessage<bool>
    {
        public IncidentPageBodyMapChangedMessage(bool value) : base(value)
        {
        }
    }

    public class PreVisitMonitorMessage : ValueChangedMessage<bool>
    {
        public PreVisitMonitorMessage(bool value) : base(value)
        {
        }
    }

    public class DataRetentionMessage : ValueChangedMessage<bool>
    {
        public DataRetentionMessage(bool value) : base(value)
        {
        }
    }

    public class PermissionsUpdatedMessage : ValueChangedMessage<bool>
    {
        public PermissionsUpdatedMessage(bool value) : base(value)
        {
        }
    }
}