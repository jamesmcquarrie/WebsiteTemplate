namespace BeyondMassages.Web.Features.Contact.Common;

public static class StatusMessages
{
    public const string SuccessMessage = "Email has been sent successfully";
    public const string AuthenticationFailed = "Client not authenticated to server";
    public const string OperationCancelled = "Email sending operation was cancelled";
    public const string SmtpCommandError = "There was an error sending the email. Please try again later";
    public const string GeneralError = "There was an unexpected error while sending the email. Please try again later";
}
