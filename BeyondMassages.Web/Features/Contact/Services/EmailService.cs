using BeyondMassages.Web.Features.Contact.Configuration;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Exceptions;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace BeyondMassages.Web.Features.Contact.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly ISmtpClient _smtpClient;

    public EmailService(ILogger<EmailService> logger, IOptions<EmailOptions> emailOptions, ISmtpClient smtpClient)
    {
        _logger = logger;
        _emailOptions = emailOptions;
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(EmailModel emailModel)
    {
        var email = ConstructEmail(emailModel);

        try
        {
            await _smtpClient.ConnectAsync(_emailOptions.Value.Host,
                _emailOptions.Value.Port,
                SecureSocketOptions.StartTls);

            await _smtpClient.AuthenticateAsync(_emailOptions.Value.UserName,
                _emailOptions.Value.Password);

            await _smtpClient.SendAsync(email);

            _logger.LogInformation("Email sent successfully");

        }

        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command error while sending email: {Message}, StatusCode: {StatusCode}", ex.Message, ex.StatusCode);
            throw new UserFriendlyException("There was an error sending the email. Please try again later", ex);
        }

        catch (Exception ex)
        {
            // A general exception occurred (could be network issues or others).
            // Log or handle the exception message and maybe the InnerException here.
            _logger.LogError(ex,"Error occurred while sending email: {Message}", ex.Message);
            throw new UserFriendlyException("There was an unexpected error while sending the email. Please try again later", ex);
        }

        finally
        {
            await _smtpClient.DisconnectAsync(true);
        }
    }

    private MimeMessage ConstructEmail(EmailModel emailModel)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailModel.EmailAddress));
        email.To.Add(MailboxAddress.Parse(_emailOptions.Value.UserName));
        email.Subject = emailModel.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = emailModel.Message };

        return email;
    }
}
