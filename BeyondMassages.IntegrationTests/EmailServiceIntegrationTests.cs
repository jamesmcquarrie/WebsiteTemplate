using BeyondMassages.Web.Features.Contact.Common;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Services;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace BeyondMassages.IntegrationTests;

public class EmailServiceIntegrationTests
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailService> _logger = Substitute.For<ILogger<EmailService>>();
    private readonly IOptions<EmailOptions> _emailOptions = Substitute.For<IOptions<EmailOptions>>();
    private readonly IAsyncPolicy _policy;
    private readonly ISmtpClient _smtpClient;

    public EmailServiceIntegrationTests()
    {
        _policy = Policy
            .Handle<SmtpCommandException>(ex => (int)ex.StatusCode >= 400 && (int)ex.StatusCode <= 500)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));

        _smtpClient = new SmtpClient();

        _emailService = new EmailService(_logger,
            _emailOptions,
            _smtpClient,
            _policy
        );
    }

    [Fact]
    public async Task SendEmailAsync_EmailShouldBeSentSuccessfully()
    {
        //Arrange
        _emailOptions.Value.Returns(new EmailOptions
        {
            UserName = "f4ed4305706a43",
            Password = "387ffa2c63fdae",
            Host = "sandbox.smtp.mailtrap.io",
            Port = 587,
            SecureSocketOptions = "StartTls"
        });

        var emailModel = new EmailModel
        {
            Name = "Test User",
            EmailAddress = "testuser@testdomain.com",
            Subject = "Test Subject",
            Message = "<p>Dear Test Admin,</p><p>This is a test message</p><p>Many Thanks,<br>Test User</p>",
        };

        //Act
        var result = await _emailService.SendEmailAsync(emailModel);

        //Assert
        Assert.True(result.IsSent);
        Assert.Equal(StatusMessages.SuccessMessage, result.Message);
    }
}