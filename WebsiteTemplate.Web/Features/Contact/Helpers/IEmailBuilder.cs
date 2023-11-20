using WebsiteTemplate.Web.Features.Contact.Models;
using MimeKit;

namespace WebsiteTemplate.Web.Features.Contact.Helpers;

public interface IEmailBuilder
{
    MimeMessage CreateMultipartEmail(EmailModel emailModel);
}
