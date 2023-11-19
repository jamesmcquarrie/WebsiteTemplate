using BeyondMassages.Web.Features.Contact.Helpers;
using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BeyondMassages.UnitTests.Helpers;

public static class EmailBuilderUnitTestsHelper
{
    public static IEmailBuilder CreateEmailBuilder(IOptions<EmailOptions> emailOptions)
    {
        var logger = CreateEmailBuilderLogger();
        return new EmailBuilder(logger, emailOptions);
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

    private static ILogger<EmailBuilder> CreateEmailBuilderLogger()
    {
        return Substitute.For<ILogger<EmailBuilder>>();
    }
}
