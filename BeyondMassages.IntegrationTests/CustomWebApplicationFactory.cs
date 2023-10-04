using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using MailKit.Net.Smtp;
using MimeKit;

namespace BeyondMassages.IntegrationTests;

public class CustomWebApplicationFactory :  WebApplicationFactory<Program>
{
    public CustomWebApplicationFactory() { }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services =>
        {
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                });

            var mockSmtpClient = Substitute.For<ISmtpClient>();
            await mockSmtpClient.SendAsync(Arg.Any<MimeMessage>());

            services.AddSingleton(mockSmtpClient);
        });
    }
}
