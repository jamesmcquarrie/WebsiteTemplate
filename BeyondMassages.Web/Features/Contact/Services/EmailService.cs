using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Polly;

namespace BeyondMassages.Web.Features.Contact.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly ISmtpClient _smtpClient;
    private readonly IAsyncPolicy _policy; 

    public EmailService(ILogger<EmailService> logger, 
        IOptions<EmailOptions> emailOptions, 
        ISmtpClient smtpClient,
        IAsyncPolicy policy)
    {
        _logger = logger;
        _emailOptions = emailOptions;
        _smtpClient = smtpClient;
        _policy = policy;
    }

    public async Task SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default)
    {
        var email = ConstructEmail(emailModel);

        try
        {
            await _policy.ExecuteAsync(async ct =>
            {
                await _smtpClient.ConnectAsync(_emailOptions.Value.Host,
                    _emailOptions.Value.Port,
                    Enum.Parse<SecureSocketOptions>(
                        _emailOptions.Value.SecureSocketOptions),
                    ct);

                await _smtpClient.AuthenticateAsync(_emailOptions.Value.UserName,
                    _emailOptions.Value.Password,
                    ct);

                await _smtpClient.SendAsync(email,
                    ct);

                _logger.LogInformation("Email has been sent successfully");
            }, cancellationToken);
        }

        catch (OperationCanceledException)
        {
            _logger.LogWarning("Email sending operation was cancelled");
            throw;
        }

        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command error while sending email: {Message}, StatusCode: {StatusCode}", ex.Message, ex.StatusCode);
            throw new UserFriendlyException("There was an error sending the email. Please try again later");
        }

        catch (Exception ex)
        {
            // A general exception occurred (could be network issues or others).
            _logger.LogError(ex,"Error occurred while sending email: {Message}", ex.Message);
            throw new UserFriendlyException("There was an unexpected error while sending the email. Please try again later");
        }

        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }

    private MimeMessage ConstructEmail(EmailModel emailModel)
    {
        CheckEmailValid(emailModel);

        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(emailModel.EmailAddress));
        email.To.Add(MailboxAddress.Parse(_emailOptions.Value.UserName));
        email.Subject = $"{emailModel.Name} - {emailModel.Subject}";
        email.Body = new TextPart(TextFormat.Html) { Text = emailModel.Message };

        return email;
    }

    private void CheckEmailValid(EmailModel emailModel) 
    {
        if (string.IsNullOrEmpty(emailModel.EmailAddress) || 
            !new EmailAddressAttribute().IsValid(emailModel.EmailAddress))
        {
            throw new ArgumentException("Invalid email");
        }

        if (string.IsNullOrEmpty(emailModel.Subject))
        {
            throw new ArgumentException("Invalid subject");
        }

        if (string.IsNullOrEmpty(emailModel.Message))
        {
            throw new ArgumentException("Invalid message body");
        }
    }
}