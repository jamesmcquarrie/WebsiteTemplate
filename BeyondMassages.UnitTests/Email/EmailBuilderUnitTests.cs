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
        var emailOptions = EmailBuilderUnitTestsHelper.GetEmailOptions();
        var emailModel = EmailBuilderUnitTestsHelper.CreateEmailModel();
        var emailBuilder = EmailBuilderUnitTestsHelper.CreateEmailBuilder(emailOptions);

        //Act
        var email = emailBuilder.CreateMultipartEmail(emailModel);

        //Assert
        Assert.Equal(emailModel.Message, email.TextBody);
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
        Assert.Equal(expectedHtmlMessage, email.HtmlBody);
    }
}
