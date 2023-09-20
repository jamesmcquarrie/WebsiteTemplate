using BeyondMassages.Web.Features.Contact.Models;

namespace BeyondMassages.Web.Features.Contact.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default);
}
