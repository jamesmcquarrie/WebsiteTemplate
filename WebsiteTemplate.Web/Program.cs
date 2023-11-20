using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Options;
using WebsiteTemplate.Web.Features.Contact.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Joonasw.AspNetCore.SecurityHeaders;
using Polly;
using Polly.Contrib.WaitAndRetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(30);
});

builder.Services.AddOptions<EmailOptions>()
    .BindConfiguration("EmailOptions")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailBuilder, EmailBuilder>();
builder.Services.AddScoped<ISmtpClient, SmtpClient>();

var policy = Policy
    .Handle<SmtpCommandException>(ex => (int)ex.StatusCode >= 400 && (int)ex.StatusCode <= 500)
    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));

builder.Services.AddSingleton<IAsyncPolicy>(policy);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Http-Error/{0}");

    app.UseHsts();
    app.UseXFrameOptions();
    app.UseXContentTypeOptions();
    app.UseXXssProtection();
    app.UseReferrerPolicy();

    app.UseCsp(options =>
    {
        options.ByDefaultAllow
            .FromSelf();

        options.AllowScripts
            .FromSelf();

        options.AllowStyles
            .FromSelf()
            .From("https://cdn.jsdelivr.net")
            .From("https://fonts.googleapis.com");

        options.AllowFonts
            .FromSelf()
            .From("https://cdn.jsdelivr.net")
            .From("https://fonts.googleapis.com")
            .From("https://fonts.gstatic.com");

        options.AllowImages
            .FromSelf()
            .From("data: https://www.w3.org");        
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { }