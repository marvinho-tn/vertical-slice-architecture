using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Notification.Api.Features;

internal sealed class EmailConfig
{
    public string Sender { get; set; }
    public string User { get; set; }
}

internal interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string content);
}

internal class EmailService(
    IOptions<EmailConfig> emailConfig,
    ILogger<EmailService> logger,
    ISendGridClient client) : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string content)
    {
        logger.LogInformation($"Sending email to {toEmail}");
        
        var from = new EmailAddress(emailConfig.Value.Sender, emailConfig.Value.User);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, content);

        await client.SendEmailAsync(msg);
    }
}

internal static class EmailExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailConfig>(configuration.GetSection("Email"));
        services.AddTransient<ISendGridClient>(sp =>
        {
            var apiKey = configuration.GetSection("SendGrid:ApiKey").Value;

            return new SendGridClient(apiKey);
        });
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }

    public static string GetEmailTemplate(this object model, string templatePath)
    {
        var template = File.ReadAllText(templatePath);

        foreach (var prop in model.GetType().GetProperties())
        {
            template = template.Replace($"@Model.{prop.Name}", prop.GetValue(model)?.ToString());
        }

        return template;
    }
}