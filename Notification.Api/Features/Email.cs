using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Notification.Api.Features;

internal sealed class EmailConfig
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Sender { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}

internal interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}

internal class EmailService(IOptions<EmailConfig> emailConfig) : IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var email = new MimeMessage();
        
        email.From.Add(new MailboxAddress(emailConfig.Value.Sender, emailConfig.Value.User));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };
        
        email.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(emailConfig.Value.Server, emailConfig.Value.Port, false);
            await client.AuthenticateAsync(emailConfig.Value.User, emailConfig.Value.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
    }
}

internal static class EmailExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailConfig>(configuration.GetSection("Email"));
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