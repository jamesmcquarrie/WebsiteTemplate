using WebsiteTemplate.Web.Features.Contact.Models;

namespace WebsiteTemplate.Web.Features.Contact.Services;

public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default);
}
