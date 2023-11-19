using BeyondMassages.UnitTests.Helpers;
using BeyondMassages.Web.Features.Contact.Common;
using MailKit.Net.Smtp;
using MimeKit;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BeyondMassages.UnitTests.Email;

public class EmailServiceUnitTests
{
    [Fact]
    public async Task SendEmailAsync_WithValidEmail_SendsSuccessfully()
    {
        //Arrange
        var smtpClient = EmailServiceUnitTestsHelper.CreateSmtpClient();
        var policy = EmailServiceUnitTestsHelper.CreatePolicy();
        var emailModel = EmailServiceUnitTestsHelper.CreateEmailModel();
        var emailService = EmailServiceUnitTestsHelper.CreateEmailService(smtpClient, policy);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        await smtpClient.Received().SendAsync(Arg.Any<MimeMessage>());
        result.IsSent.Should().BeTrue();
        result.Message.Should().Be(StatusMessages.SuccessMessage);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesSmtpCommandException_SendsUnsuccessfully()
    {
        //Arrange
        var smtpClient = EmailServiceUnitTestsHelper.CreateSmtpClient();
        var policy = EmailServiceUnitTestsHelper.CreatePolicy();
        var emailModel = EmailServiceUnitTestsHelper.CreateEmailModel();
        var emailService = EmailServiceUnitTestsHelper.CreateEmailService(smtpClient, policy);

        var smtpCommandException = new SmtpCommandException(SmtpErrorCode.MessageNotAccepted, SmtpStatusCode.InsufficientStorage, StatusMessages.SmtpCommandError);
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(smtpCommandException);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        result.IsSent.Should().BeFalse();
        result.Message.Should().Be(StatusMessages.SmtpCommandError);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesOperationCanceledException_SendsUnsuccessfully()
    {
        //Arrange
        var smtpClient = EmailServiceUnitTestsHelper.CreateSmtpClient();
        var policy = EmailServiceUnitTestsHelper.CreatePolicy();
        var emailModel = EmailServiceUnitTestsHelper.CreateEmailModel();
        var emailService = EmailServiceUnitTestsHelper.CreateEmailService(smtpClient, policy);

        var operationCanceledException = new OperationCanceledException();
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(operationCanceledException);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        result.IsSent.Should().BeFalse();
        result.Message.Should().Be(StatusMessages.OperationCancelled);
    }

    [Fact]
    public async Task SendEmailAsync_RaisesGeneralException_SendsUnsuccessfully()
    {
        //Arrange
        var smtpClient = EmailServiceUnitTestsHelper.CreateSmtpClient();
        var policy = EmailServiceUnitTestsHelper.CreatePolicy();
        var emailModel = EmailServiceUnitTestsHelper.CreateEmailModel();
        var emailService = EmailServiceUnitTestsHelper.CreateEmailService(smtpClient, policy);

        var exception = new Exception(StatusMessages.GeneralError);
        smtpClient.SendAsync(Arg.Any<MimeMessage>())
            .ThrowsAsync(exception);

        //Act
        var result = await emailService.SendEmailAsync(emailModel);

        //Assert
        result.IsSent.Should().BeFalse();
        result.Message.Should().Be(StatusMessages.GeneralError);
    }
}