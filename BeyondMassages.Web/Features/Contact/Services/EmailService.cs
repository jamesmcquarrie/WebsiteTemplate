using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Common;
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

    public async Task<EmailResult> SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default)
    {
        var email = ConstructEmail(emailModel);
        var emailResult = new EmailResult();

        try
        {
            await _policy.ExecuteAsync(async ct =>
            {
                await ConnectAsync(ct);
                await AuthenticateAsync(ct);

                await _smtpClient.SendAsync(email,
                    ct);

                emailResult.IsSent = true;
                emailResult.Message = StatusMessages.SuccessMessage;

                _logger.LogInformation("Email has been sent successfully");
            }, cancellationToken);

            return emailResult;
        }

        catch (AuthenticationException ex)
        {
            _logger.LogCritical(ex, "Client not authenticated to server", ex.Message);
            throw;
        }

        catch (OperationCanceledException)
        {
            _logger.LogWarning("Email sending operation was cancelled");
            emailResult.Message = StatusMessages.OperationCancelled;

            return emailResult;
        }

        catch (SmtpCommandException ex)
        {
            _logger.LogError(ex, "SMTP command error while sending email: {Message}, StatusCode: {StatusCode}", ex.Message, ex.StatusCode);
            emailResult.Message = StatusMessages.SmtpCommandError;

            return emailResult;
        }

        catch (Exception ex)
        {
            // A general exception occurred (could be network issues or others).
            _logger.LogError(ex,"Error occurred while sending email: {Message}", ex.Message);
            emailResult.Message = StatusMessages.GeneralError;

            return emailResult;
        }

        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }

    private async Task ConnectAsync(CancellationToken ct)
    {
        await _smtpClient.ConnectAsync(
            _emailOptions.Value.Host,
            _emailOptions.Value.Port,
            Enum.Parse<SecureSocketOptions>(
                _emailOptions.Value.SecureSocketOptions),
            ct);
    }

    private async Task AuthenticateAsync(CancellationToken ct)
    {
        await _smtpClient.AuthenticateAsync(
            new SaslMechanismCramMd5(
                _emailOptions.Value.UserName,
                _emailOptions.Value.Password),
            ct);
    }

    private MimeMessage ConstructEmail(EmailModel emailModel)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(emailModel.EmailAddress));
        email.To.Add(MailboxAddress.Parse(_emailOptions.Value.UserName));
        email.Subject = $"{emailModel.Name} - {emailModel.Subject}";
        email.Body = new TextPart(TextFormat.Html) { Text = emailModel.Message };

        return email;
    }
}