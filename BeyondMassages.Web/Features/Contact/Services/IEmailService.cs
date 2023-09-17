using BeyondMassages.Web.Features.Contact.Models;
using MimeKit;

namespace BeyondMassages.Web.Features.Contact.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailModel emailModel);
}
