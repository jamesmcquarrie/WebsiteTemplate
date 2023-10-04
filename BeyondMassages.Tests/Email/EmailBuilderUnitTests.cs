using BeyondMassages.Web.Features.Contact.Helpers;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Options;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BeyondMassages.UnitTests.Email;

public class EmailBuilderUnitTests
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

    private IEmailBuilder CreateEmailBuilder(IOptions<EmailOptions> emailOptions)
    {
        return new EmailBuilder(emailOptions);
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedEmailAttributes()
    {
        //Arrange
        var emailOptions = GetEmailOptions();
        var emailModel = CreateEmailModel();
        var emailBuilder = CreateEmailBuilder(emailOptions);

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        Assert.NotNull(email);
        Assert.Equal(emailOptions.Value.IntermediaryEmailAddress, email.From.ToString());
        Assert.Equal(emailOptions.Value.UserName, email.To.ToString());
        Assert.Equal(emailModel.EmailAddress, email.ReplyTo.ToString());
        Assert.Equal($"{emailModel.Name} ({emailModel.EmailAddress}) - {emailModel.Subject}", email.Subject.ToString());
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedTextBody()
    {
        //Arrange
        var emailOptions = GetEmailOptions();
        var emailModel = CreateEmailModel();
        var emailBuilder = CreateEmailBuilder(emailOptions);

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        Assert.Equal(emailModel.Message, email.TextBody);
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedHtmlBody()
    {
        //Arrange
        var emailOptions = GetEmailOptions();
        var emailModel = CreateEmailModel();
        var emailBuilder = CreateEmailBuilder(emailOptions);
        var expectedHtmlMessage = emailModel.Message.ReplaceLineEndings( "<br>");

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        Assert.Equal(expectedHtmlMessage, email.HtmlBody);
    }
}
