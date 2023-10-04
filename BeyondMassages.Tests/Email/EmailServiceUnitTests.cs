using BeyondMassages.Web.Features.Contact.Common;
using BeyondMassages.Web.Features.Contact.Helpers;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace BeyondMassages.UnitTests.Email;

public class EmailServiceUnitTests
{
    private IOptions<EmailOptions> GetEmailOptions()
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

    private EmailModel CreateEmailModel()
    {
        return new EmailModel
        {
            Name = "Test User",
            EmailAddress = "testuser@testuserdomain.com",
            Subject = "Test Subject",
            Message = "Dear Admin,\r\n\r\nThis is a test message.\r\n\r\nMany Thanks,\r\nTest User",
        };
    }

    private IAsyncPolicy CreatePolicy()
    {
        return Policy
            .Handle<SmtpCommandException>(ex => (int)ex.StatusCode >= 400 && (int)ex.StatusCode <= 500)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));
    }

    private ILogger<EmailService> CreateLogger()
    {
        return Substitute.For<ILogger<EmailService>>();
    }

    private ISmtpClient CreateSmtpClient()
    {
        return Substitute.For<ISmtpClient>();
    }

    private IEmailBuilder CreateEmailBuilder(IOptions<EmailOptions> emailOptions)
    {
        return new EmailBuilder(emailOptions);
    }

    private IEmailService CreateEmailService(ISmtpClient smtpClient, IAsyncPolicy policy)
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

    [Fact]
    public async Task SendEmailAsync_WithValidEmail_SendsSuccessfully()
    {
        //Arrange
        var smtpClient = CreateSmtpClient();
        var policy = CreatePolicy();    
        var emailService = CreateEmailService(smtpClient, policy);

        var emailModel = CreateEmailModel();

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        await smtpClient.Received().SendAsync(Arg.Any<MimeMessage>());
        Assert.True(result.IsSent);
        Assert.Equal(StatusMessages.SuccessMessage, result.Message);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesSmtpCommandException_SendsUnsuccessfully()
    {
        //Arrange
        var smtpClient = CreateSmtpClient();
        var policy = CreatePolicy();    
        var emailService = CreateEmailService(smtpClient, policy);

        var emailModel = CreateEmailModel();

        var smtpCommandException = new SmtpCommandException(SmtpErrorCode.MessageNotAccepted, SmtpStatusCode.InsufficientStorage, StatusMessages.SmtpCommandError);
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(smtpCommandException);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        Assert.False(result.IsSent);
        Assert.Equal(StatusMessages.SmtpCommandError, result.Message);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesOperationCanceledException_SendsUnsuccessfully()
    {
        //Arrange
        var smtpClient = CreateSmtpClient();
        var policy = CreatePolicy();
        var emailService = CreateEmailService(smtpClient, policy);

        var emailModel = CreateEmailModel();

        var operationCanceledException = new OperationCanceledException();
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(operationCanceledException);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        Assert.False(result.IsSent);
        Assert.Equal(StatusMessages.OperationCancelled, result.Message);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesGeneralException_SendsUnsuccessfully()
    {
        //Arrange
        var smtpClient = CreateSmtpClient();
        var policy = CreatePolicy();
        var emailService = CreateEmailService(smtpClient, policy);

        var emailModel = CreateEmailModel();

        var exception = new Exception(StatusMessages.GeneralError);
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(exception);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        Assert.False(result.IsSent);
        Assert.Equal(StatusMessages.GeneralError, result.Message);
    }
}