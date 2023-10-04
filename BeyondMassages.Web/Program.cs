using BeyondMassages.Web.Features.Contact.Options;
using BeyondMassages.Web.Features.Contact.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
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

// https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-7.0
//builder.Services.AddHsts(options =>
//{
//    options.Preload = true;
//    options.IncludeSubDomains = true;
//    options.MaxAge = TimeSpan.FromDays(365);
//    options.ExcludedHosts.Add("example.com");
//    options.ExcludedHosts.Add("www.example.com");
//});

// https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-7.0
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigins",
//                      builder =>
//                      {
//                          builder.WithOrigins("http://example.com",
//                                              "https://another-example.com")
//                                 .AllowAnyHeader()
//                                 .AllowAnyMethod();
//                      });
//});

builder.Services.AddOptions<EmailOptions>()
    .BindConfiguration("EmailOptions")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IEmailService, EmailService>();
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.Use(async (context, next) =>
    {
        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; " +
                "style-src 'self' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
                "font-src 'self' https://cdn.jsdelivr.net https://fonts.googleapis.com https://fonts.gstatic.com; " +
                "img-src 'self' data: https://www.w3.org");
        }
        await next();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { }