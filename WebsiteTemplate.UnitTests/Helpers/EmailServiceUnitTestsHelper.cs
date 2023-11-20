﻿using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Options;
using WebsiteTemplate.Web.Features.Contact.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using Polly.Contrib.WaitAndRetry;
using Polly;
using NSubstitute;

namespace WebsiteTemplate.UnitTests.Helpers;

public static class EmailServiceUnitTestsHelper
{
    public static IEmailService CreateEmailService(ISmtpClient smtpClient, IAsyncPolicy policy)
    {
        var logger = CreateEmailServiceLogger();
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

    private static IOptions<EmailOptions> GetEmailOptions()
    {
        var emailOptions = Substitute.For<IOptions<EmailOptions>>();
        emailOptions.Value.Returns(new EmailOptions()
        {
            UserName = "alphonso.lockman85@ethereal.email",
            Password = "ww8GnByJspJucUghX9",
            Host = "smtp.ethereal.email",
            Port = 587,
            SecureSocketOptions = "StartTls"
        });

        return emailOptions;
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

    private static IEmailBuilder CreateEmailBuilder(IOptions<EmailOptions> emailOptions)
    {
        var logger = CreateEmailBuilderLogger();
        return new EmailBuilder(logger, emailOptions);
    }

    private static ILogger<EmailService> CreateEmailServiceLogger()
    {
        return Substitute.For<ILogger<EmailService>>();
    }

    private static ILogger<EmailBuilder> CreateEmailBuilderLogger()
    {
        return Substitute.For<ILogger<EmailBuilder>>();
    }
}