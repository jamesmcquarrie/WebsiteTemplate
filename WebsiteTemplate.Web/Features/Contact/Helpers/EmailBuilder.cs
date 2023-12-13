using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Options;
using Microsoft.Extensions.Options;
using MimeKit;

namespace WebsiteTemplate.Web.Features.Contact.Helpers;

public class EmailBuilder
{
    private readonly EmailOptions _emailOptions;

    public EmailBuilder(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public MimeMessage CreateMultipartEmail(EmailModel emailModel)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(emailModel.EmailAddress));
        email.To.Add(MailboxAddress.Parse(_emailOptions.UserName));
        email.ReplyTo.Add(MailboxAddress.Parse(emailModel.EmailAddress));
        email.Subject = $"{emailModel.Name} ({emailModel.EmailAddress}) - {emailModel.Subject}";
        email.Body = CreateBodyBuilder(emailModel.Message)
            .ToMessageBody();

        return email;
    }

    private BodyBuilder CreateBodyBuilder(string message)
    {
        var bodyBuilder = new BodyBuilder()
        {
            TextBody = message,
            HtmlBody = message.ReplaceLineEndings("<br>")
        };

        return bodyBuilder;
    }
}
