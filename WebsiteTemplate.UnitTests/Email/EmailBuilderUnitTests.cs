using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Options;
using Microsoft.Extensions.Options;

namespace WebsiteTemplate.UnitTests.Email;

public class EmailBuilderUnitTests
{
    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedEmailAttributes()
    {
        //Arrange
        var fixture = new Fixture();

        var emailOptions = Substitute.For<IOptions<EmailOptions>>();
        emailOptions.Value
           .Returns(fixture.Create<EmailOptions>());

        var emailBuilder = new EmailBuilder(emailOptions);

        var emailModel = fixture.Create<EmailModel>();

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        email.From.ToString().Should().Be(emailModel.EmailAddress);
        email.To.ToString().Should().Be(emailOptions.Value.UserName);
        email.ReplyTo.ToString().Should().Be(emailModel.EmailAddress);
        email.Subject.ToString().Should().Be($"{emailModel.Name} ({emailModel.EmailAddress}) - {emailModel.Subject}");
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedTextBody()
    {
        //Arrange
        var fixture = new Fixture();

        var emailOptions = Substitute.For<IOptions<EmailOptions>>();
        emailOptions.Value
           .Returns(fixture.Create<EmailOptions>());

        var emailBuilder = new EmailBuilder(emailOptions);

        var emailModel = fixture.Create<EmailModel>();

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        email.TextBody.Should().Be(emailModel.Message);
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedHtmlBody()
    {
        //Arrange
        var fixture = new Fixture();

        var emailOptions = Substitute.For<IOptions<EmailOptions>>();
        emailOptions.Value
           .Returns(fixture.Create<EmailOptions>());

        var emailBuilder = new EmailBuilder(emailOptions);

        var emailModel = fixture.Create<EmailModel>();
        emailModel.Message = $"{Environment.NewLine} {emailModel.Message} {Environment.NewLine}";

        var expectedHtmlMessage = emailModel.Message.ReplaceLineEndings( "<br>");

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        email.HtmlBody.Should().Be(expectedHtmlMessage);
    }
}
