using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Helpers;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Common;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Polly;

namespace BeyondMassages.Web.Features.Contact.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailBuilder _emailBuilder;
    private readonly EmailOptions _emailOptions;
    private readonly ISmtpClient _smtpClient;
    private readonly IAsyncPolicy _policy; 

    public EmailService(ILogger<EmailService> logger,
        IEmailBuilder emailBuilder,
        IOptions<EmailOptions> emailOptions, 
        ISmtpClient smtpClient,
        IAsyncPolicy policy)
    {
        _logger = logger;
        _emailBuilder = emailBuilder;
        _emailOptions = emailOptions.Value;
        _smtpClient = smtpClient;
        _policy = policy;
    }

    public async Task<EmailResult> SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default)
    {
        var email = _emailBuilder.CreateMultipartEmail(emailModel);
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
            _emailOptions.Host,
            _emailOptions.Port,
            Enum.Parse<SecureSocketOptions>(
                _emailOptions.SecureSocketOptions),
            ct);
    }

    private async Task AuthenticateAsync(CancellationToken ct)
    {
        await _smtpClient.AuthenticateAsync(
            new SaslMechanismCramMd5(
                _emailOptions.UserName,
                _emailOptions.Password),
            ct);
    }
}