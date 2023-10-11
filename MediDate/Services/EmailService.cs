using MediDate.Models.Queries;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MediDate.Models;
using NuGet.Common;

namespace MediDate.Services
{
    public class EmailService : IEmailService
    {
        internal string UserName;
        internal string PassWord;

        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendMail(Correo request)
        {
            UserName = Environment.GetEnvironmentVariable("UsarName");
            PassWord = Environment.GetEnvironmentVariable("PassWord");

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(UserName));
            email.To.Add(MailboxAddress.Parse(request.Destinatario));
            email.Subject = request.Asunto;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Mensaje
            };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _config.GetSection("Email:Host").Value,
               Convert.ToInt32(_config.GetSection("Email:Port").Value),
               SecureSocketOptions.StartTls
                );


            smtp.Authenticate(UserName, PassWord);

            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
