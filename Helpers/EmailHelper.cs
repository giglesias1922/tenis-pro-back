using MimeKit;
using MailKit.Net.Smtp; // ESTE using es el que faltaba

namespace tenis_pro_back.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var MailHeader = _configuration["EMail:Header"];
            var MailNoReply = _configuration["EMail:NoReply"];
            var MailFrom = _configuration["EMail:From"];
            var MailSmtp = _configuration["EMail:Smtp"];
            var MailPassword = _configuration["EMail:Password"];
            int MailSmtpPort = Convert.ToInt32(_configuration["EMail:SmtpPort"]);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailHeader, MailNoReply));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(MailSmtp, MailSmtpPort, false);
            await client.AuthenticateAsync(MailFrom, MailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}