using BeyondMassages.Web.Features.Contact.Helpers;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using Polly.Contrib.WaitAndRetry;
using Polly;
using NSubstitute;

namespace BeyondMassages.UnitTests.Helpers;

public static class EmailServiceUnitTestsHelper
{
    public static IOptions<EmailOptions> GetEmailOptions()
    {
        var emailOptions = Substitute.For<IOptions<EmailOptions>>();
        emailOptions.Value.Returns(new EmailOptions()
        {
            UserName = "f4ed4305706a43",
            Password = "387ffa2c63fdae",
            IntermediaryEmailAddress = "hello@testdomain.com",
            Host = "sandbox.smtp.mailtrap.io",
            Port = 587,
            SecureSocketOptions = "StartTls"
        });

        return emailOptions;
    }

    public static ILogger<EmailService> CreateLogger()
    {
        return Substitute.For<ILogger<EmailService>>();
    }

    public static ISmtpClient CreateSmtpClient()
    {
        return Substitute.For<ISmtpClient>();
    }

    public static IAsyncPolicy CreatePolicy()
    {
        return Policy
            .Handle<SmtpCommandException>(ex => (int)ex.StatusCode >= 400 && (int)ex.StatusCode <= 500)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));
    }

    public static IEmailBuilder CreateEmailBuilder(IOptions<EmailOptions> emailOptions)
    {
        return new EmailBuilder(emailOptions);
    }

    public static IEmailService CreateEmailService(ISmtpClient smtpClient, IAsyncPolicy policy)
    {
        var logger = CreateLogger();
        var emailOptions = GetEmailOptions();
        var emailBuilder = CreateEmailBuilder(emailOptions);

        return new EmailService(logger,
            emailBuilder,
            emailOptions,
            smtpClient,
            policy);
    }

    public static EmailModel CreateEmailModel()
    {
        return new EmailModel
        {
            Name = "Test User",
            EmailAddress = "testuser@testuserdomain.com",
            Subject = "Test Subject",
            Message = "Dear Admin,\r\n\r\nThis is a test message.\r\n\r\nMany Thanks,\r\nTest User",
        };
    }
}
