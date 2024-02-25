using WebsiteTemplate.Web.Features.Contact.Helpers;
using WebsiteTemplate.Web.Features.Contact.Options;
using WebsiteTemplate.Web.Features.Contact.Services;
using WebsiteTemplate.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Joonasw.AspNetCore.SecurityHeaders;
using MailKit.Net.Smtp;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
});

builder.Services.AddRazorPages(options => options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(30);
});

builder.Services.AddOptions<EmailOptions>()
    .BindConfiguration(EmailOptions.OptionsName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddResiliencePipeline(ResilienceStrategies.SmtpCommandPolicy, pipelineBuilder =>
{
    pipelineBuilder.AddRetry(new()
    {
        MaxRetryAttempts = 3,
        ShouldHandle = new PredicateBuilder()
            .Handle<SmtpCommandException>(ex => (int)ex.StatusCode >= 400 && (int)ex.StatusCode <= 500),
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true
    });
});

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailBuilder>();
builder.Services.AddScoped<SmtpClient>();

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
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { }