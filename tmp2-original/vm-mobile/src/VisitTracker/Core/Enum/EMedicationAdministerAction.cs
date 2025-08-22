namespace VisitTracker;

/// <summary>
/// EMedicationAdministerAction is an enumeration that defines the different actions that can be taken when administering medication to a patient.
/// Each action corresponds to a specific step in the medication administration process, such as administering the medication, seeking approval, or awaiting confirmation.
/// </summary>
public enum EMedicationAdministerAction
{
    [Description("Administer")]
    Administer = 0,

    [Description("Seek Approval")]
    SeekApproval = 1,

    [Description("Awaiting Confirmation")]
    AwaitingConfirmation = 2,

    [Description("Proceed")]
    Proceed = 3,

    [Description("Do Not Proceed")]
    DoNotProceed = 4,

    [Description("No Response")]
    NoResponse = 5
}