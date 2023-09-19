using BeyondMassages.Web.Features.Contact.Configuration;
using BeyondMassages.Web.Features.Contact.Services;
using MailKit.Net.Smtp;
using Polly.Contrib.WaitAndRetry;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddRazorPages();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.ConfigureOptions<EmailOptionsSetup>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<ISmtpClient, SmtpClient>();

var policy = Policy
    .Handle<SmtpCommandException>(ex => ex.ErrorCode == SmtpErrorCode.RecipientNotAccepted)
    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));

builder.Services.AddSingleton<IAsyncPolicy>(policy);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
