using BeyondMassages.Web.Features.Contact.Models;
using MimeKit;

namespace BeyondMassages.Web.Features.Contact.Helpers;

public interface IEmailBuilder
{
    MimeMessage CreateMultipartEmail(EmailModel emailModel);
}
