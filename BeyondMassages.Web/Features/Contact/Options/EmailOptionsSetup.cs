using Microsoft.Extensions.Options;

namespace BeyondMassages.Web.Features.Contact.Configuration
{
    public class EmailOptionsSetup : IConfigureOptions<EmailOptions>
    {
        private readonly IConfiguration _config;
        public EmailOptionsSetup(IConfiguration config) 
        {
            _config = config;
        }

        public void Configure(EmailOptions options)
        {
            _config.GetSection(nameof(EmailOptions))
                .Bind(options);
        }
    }
}
