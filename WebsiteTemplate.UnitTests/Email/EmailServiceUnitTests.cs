using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Options;
using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Services;
using WebsiteTemplate.Web.Features.Contact.Constants;
using WebsiteTemplate.Web.Common;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using MailKit.Net.Smtp;
using MimeKit;
using Polly;
using Polly.Registry;

namespace WebsiteTemplate.UnitTests.Email;

public class EmailServiceUnitTests
{
    [Fact]
    public async Task SendEmailAsync_WithValidEmail_SendsSuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<EmailService>>();
        var smtpClient = Substitute.For<SmtpClient>();
        var emailOptions = SetupEmailOptions();
        var emailBuilder = Substitute.For<EmailBuilder>(emailOptions);
        var pipelineProvider = SetupResiliencePipelineProvider();

        var emailService = new EmailService(logger,
            emailBuilder,
            emailOptions,
            smtpClient,
            pipelineProvider);

        var emailModel = new Fixture().Create<EmailModel>();

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        await smtpClient.Received().SendAsync(Arg.Any<MimeMessage>());
        result.IsSent.Should().BeTrue();
        result.Message.Should().Be(EmailStatusMessages.SuccessMessage);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesSmtpCommandException_SendsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<EmailService>>();
        var smtpClient = Substitute.For<SmtpClient>();
        var emailOptions = SetupEmailOptions();
        var emailBuilder = Substitute.For<EmailBuilder>(emailOptions);
        var pipelineProvider = SetupResiliencePipelineProvider();

        var emailService = new EmailService(logger,
            emailBuilder,
            emailOptions,
            smtpClient,
            pipelineProvider);

        var emailModel = new Fixture().Create<EmailModel>();

        var smtpCommandException = new SmtpCommandException(SmtpErrorCode.MessageNotAccepted, SmtpStatusCode.InsufficientStorage, EmailStatusMessages.SmtpCommandError);
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(smtpCommandException);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        result.IsSent.Should().BeFalse();
        result.Message.Should().Be(EmailStatusMessages.SmtpCommandError);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesOperationCanceledException_SendsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<EmailService>>();
        var smtpClient = Substitute.For<SmtpClient>();
        var emailOptions = SetupEmailOptions();
        var emailBuilder = Substitute.For<EmailBuilder>(emailOptions);
        var pipelineProvider = SetupResiliencePipelineProvider();

        var emailService = new EmailService(logger,
            emailBuilder,
            emailOptions,
            smtpClient,
            pipelineProvider);

        var emailModel = new Fixture().Create<EmailModel>();

        var operationCanceledException = new OperationCanceledException();
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(operationCanceledException);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        result.IsSent.Should().BeFalse();
        result.Message.Should().Be(EmailStatusMessages.OperationCancelled);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesGeneralException_SendsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<EmailService>>();
        var smtpClient = Substitute.For<SmtpClient>();
        var emailOptions = SetupEmailOptions();
        var emailBuilder = Substitute.For<EmailBuilder>(emailOptions);
        var pipelineProvider = SetupResiliencePipelineProvider();

        var emailService = new EmailService(logger,
            emailBuilder,
            emailOptions,
            smtpClient,
            pipelineProvider);

        var emailModel = new Fixture().Create<EmailModel>();

        var exception = new Exception(EmailStatusMessages.GeneralError);
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(exception);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        result.IsSent.Should().BeFalse();
        result.Message.Should().Be(EmailStatusMessages.GeneralError);
    }

    private IOptions<EmailOptions> SetupEmailOptions()
    {
        var emailOptions = Substitute.For<IOptions<EmailOptions>>();
        emailOptions.Value
            .Returns(new Fixture().Create<EmailOptions>());
        emailOptions.Value.SecureSocketOptions = "StartTls";

        return emailOptions;
    }

    private ResiliencePipelineProvider<string> SetupResiliencePipelineProvider()
    {
        var pipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();
        pipelineProvider
            .GetPipeline(ResilienceStrategies.SmtpCommandPolicy)
            .Returns(ResiliencePipeline.Empty);

        return pipelineProvider;
    }
}