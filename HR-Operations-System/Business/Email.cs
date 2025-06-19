using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace HR_Operations_System.Business
{
    public class Email
    {
        private string _sender;
        private string _password;
        public Email(string sender, string password)
        {
            _sender = sender;
            _password = password;
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            try
            {
                // Set up the email details
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_sender);
                mail.To.Add(recipient);
                mail.Subject = subject;
                mail.Body = body;

                // Configure the SMTP client
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); // Replace with your SMTP server and port
                smtp.Credentials = new NetworkCredential(_sender, _password);
                smtp.EnableSsl = true;

                // Send the email
                smtp.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
