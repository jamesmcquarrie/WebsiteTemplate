using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace WebsiteTemplate.UnitTests.Helpers;

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
            UserName = "alphonso.lockman85@ethereal.email",
            Password = "ww8GnByJspJucUghX9",
            Host = "smtp.ethereal.email",
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
