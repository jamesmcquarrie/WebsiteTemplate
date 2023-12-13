using WebsiteTemplate.Web.Features.Contact.Options;
using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Common;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Polly.Registry;
using Polly;

namespace WebsiteTemplate.Web.Features.Contact.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailBuilder _emailBuilder;
    private readonly EmailOptions _emailOptions;
    private readonly SmtpClient _smtpClient;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider; 

    public EmailService(ILogger<EmailService> logger,
        EmailBuilder emailBuilder,
        IOptions<EmailOptions> emailOptions, 
        SmtpClient smtpClient,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        _logger = logger;
        _emailBuilder = emailBuilder;
        _emailOptions = emailOptions.Value;
        _smtpClient = smtpClient;
        _pipelineProvider = pipelineProvider;
    }

    public async Task<EmailResult> SendEmailAsync(EmailModel emailModel, CancellationToken cancellationToken = default)
    {
        var email = _emailBuilder.CreateMultipartEmail(emailModel);
        _logger.LogInformation("Email has been constructed");

        var emailResult = new EmailResult();

        try
        {
            var policy = _pipelineProvider.GetPipeline(ResilienceStrategies.SmtpCommandPolicy);

            await policy.ExecuteAsync(async ct =>
            {
                await ConnectAsync(ct);
                await AuthenticateAsync(ct);

                await _smtpClient.SendAsync(email, ct);

                emailResult.IsSent = true;
                emailResult.Message = StatusMessages.SuccessMessage;

                _logger.LogInformation("Email has been sent successfully");
            }, cancellationToken); // Ensure this is the correct CancellationToken

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
                _emailOptions.UserName,
                _emailOptions.Password,
            ct);
    }
}