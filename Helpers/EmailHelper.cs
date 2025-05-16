using MimeKit;
using System.Net.Mail;

namespace tenis_pro_back.Helpers
{
    public class EmailHelper
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mi App", "no-reply@miapp.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.tuservidor.com", 587, false);
            await client.AuthenticateAsync("usuario", "contraseña");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
