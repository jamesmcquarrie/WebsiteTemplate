using BeyondMassages.Web.Features.Contact.Models;

namespace BeyondMassages.Web.Features.Contact.Services;

public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default);
}
