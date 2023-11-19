using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BeyondMassages.Web.Features.Contact.Helpers;

public class EmailBuilder : IEmailBuilder
{
    private readonly ILogger<EmailBuilder> _logger;
    private readonly EmailOptions _emailOptions;

    public EmailBuilder(ILogger<EmailBuilder> logger,
        IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
    }

    public MimeMessage CreateMultipartEmail(EmailModel emailModel)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_emailOptions.IntermediaryEmailAddress));
        email.To.Add(MailboxAddress.Parse(_emailOptions.UserName));
        email.ReplyTo.Add(MailboxAddress.Parse(emailModel.EmailAddress));
        email.Subject = $"{emailModel.Name} ({emailModel.EmailAddress}) - {emailModel.Subject}";
        email.Body = CreateBodyBuilder(emailModel.Message)
            .ToMessageBody();

        _logger.LogInformation("Email has been constructed");

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
