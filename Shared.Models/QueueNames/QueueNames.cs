namespace Shared.Models.QueueNames;

public static class QueueNames
{
    public const string PasswordResetQueue = "benkyou_password_reset";
    public const string EmailConfirmationQueue = "benkyou_email_confirmation";
    public const string RegistrationQueue = "benkyou_user_registration";
    public const string RegistrationTimeQueue = "benkyou_user_registration_time";
    public const string AccountVisibilityChangeQueue = "benkyou_user_visibility";
    public const string FinishSetLearningQueue = "benkyou_finish_learning";
}