namespace Shared.Models.QueueNames;

public static class QueueNames
{
    public const string PasswordResetQueue = "benkyou_password_reset";
    public const string EmailConfirmationQueue = "benkyou_email_confirmation";
    public const string PasswordResetQueueWithProtocol = "queue:benkyou_password_reset";
    public const string EmailConfirmationQueueWithProtocol = "queue:benkyou_email_confirmation";
}