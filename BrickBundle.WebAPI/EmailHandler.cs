using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BrickBundle.WebAPI
{
    /// <summary>
    /// Contains methods to send emails based on templates in the Emails folder.
    /// </summary>
    public static class EmailHandler
    {
        private const string fromEmail = "no-reply@brickbundle.azurewebsites.net";
        private const string fromEmailName = "BrickBundle";

        public static void SendEmailVerificationCode(string emailAddress, string verificationCode)
        {
            string subject = "Verify BrickBundle Email";
            string[] htmlLines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Emails\VerifyEmail.html"));
            string body = string.Join("", htmlLines);
            body = body.Replace("{code}", verificationCode);
            SendEmailAsync(emailAddress, body, subject).ConfigureAwait(false);
        }

        public static void SendResetPasswordCode(string emailAddress, string verificationCode)
        {
            string subject = "Reset BrickBundle Password";
            string[] htmlLines = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Emails\ResetPasswordEmail.html"));
            string body = string.Join("", htmlLines);
            body = body.Replace("{code}", verificationCode);
            SendEmailAsync(emailAddress, body, subject).ConfigureAwait(false);
        }

        private static async Task<Response> SendEmailAsync(string emailAddress, string body, string subject)
        {
            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(fromEmail, fromEmailName));
            msg.AddTo(emailAddress);
            msg.SetSubject(subject);
            msg.AddContent(MimeType.Html, body);
            var response = await client.SendEmailAsync(msg);
            // log response errors here
            return response;
        }
    }
}
