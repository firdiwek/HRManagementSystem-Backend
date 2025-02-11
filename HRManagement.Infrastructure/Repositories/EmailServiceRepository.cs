using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using HRManagement.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_emailSettings.SenderEmail);
        email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        
        // Connect to the SMTP server
        await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        
        // Authenticate with the SMTP server
        await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        
        // Send the email
        await smtp.SendAsync(email);
        
        // Disconnect from the SMTP server
        await smtp.DisconnectAsync(true);
    }
}
