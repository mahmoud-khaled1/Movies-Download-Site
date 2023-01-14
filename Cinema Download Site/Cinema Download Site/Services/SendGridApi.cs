using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace Cinema_Download_Site.Services
{
    public static class SendGridApi
    {
        public static async Task<bool> SendEmail(string userEmail,string userName
            ,string plainTextContent,string htmlContent,string subject)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "MahmoudKhaled");
            var to = new EmailAddress(userEmail, userName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            return await Task.FromResult(true);
        }
    }
}
