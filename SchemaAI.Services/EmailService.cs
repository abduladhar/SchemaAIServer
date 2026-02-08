using SchemaAI.Entities;
using MimeKit;
using MailKit.Net.Smtp;

namespace SchemaAI.Services
{
    public class EmailService
    {
        private readonly EmailSetting _emailSettings;
        public EmailService(EmailSetting emailSettings)
        {
            _emailSettings = emailSettings;
        }

        //public async Task SendEmail(string toEmail, string subject, string body)
        //{
        //    SmtpClient smtpClient = new SmtpClient(_emailSettings.SmtpHost, Convert.ToInt32(_emailSettings.SmtpPort))
        //    {
        //        EnableSsl = true,
        //        Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password)
        //    };

        //    MailMessage mail = new MailMessage
        //    {
        //        From = new MailAddress(_emailSettings.SenderEmail),
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true
        //    };

        //    mail.To.Add(toEmail);

        //    smtpClient.Send(mail);

        //}



        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; // failed
            }
        }
    }
}
