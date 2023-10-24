using BeyondMassages.UnitTests.Helpers;

namespace BeyondMassages.UnitTests.Email;

public class EmailBuilderUnitTests
{
    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedEmailAttributes()
    {
        //Arrange
        var emailOptions = EmailBuilderUnitTestsHelper.GetEmailOptions();
        var emailModel = EmailBuilderUnitTestsHelper.CreateEmailModel();
        var emailBuilder = EmailBuilderUnitTestsHelper.CreateEmailBuilder(emailOptions);

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        email.From.ToString().Should().Be(emailOptions.Value.IntermediaryEmailAddress);
        email.To.ToString().Should().Be(emailOptions.Value.UserName);
        email.ReplyTo.ToString().Should().Be(emailModel.EmailAddress);
        email.Subject.ToString().Should().Be($"{emailModel.Name} ({emailModel.EmailAddress}) - {emailModel.Subject}");
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedTextBody()
    {
        //Arrange
        var emailOptions = EmailBuilderUnitTestsHelper.GetEmailOptions();
        var emailModel = EmailBuilderUnitTestsHelper.CreateEmailModel();
        var emailBuilder = EmailBuilderUnitTestsHelper.CreateEmailBuilder(emailOptions);

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        email.TextBody.Should().Be(emailModel.Message);
    }

    [Fact]
    public void CreateMultipartEmail_GivenValidInputs_SetsExpectedHtmlBody()
    {
        //Arrange
        var emailOptions = EmailBuilderUnitTestsHelper.GetEmailOptions();
        var emailModel = EmailBuilderUnitTestsHelper.CreateEmailModel();
        var emailBuilder = EmailBuilderUnitTestsHelper.CreateEmailBuilder(emailOptions);
        var expectedHtmlMessage = emailModel.Message.ReplaceLineEndings( "<br>");

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        email.HtmlBody.Should().Be(expectedHtmlMessage);
    }
}
