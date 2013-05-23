using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CorriAndMike.Services
{
    public static class MailService
    {
        private const string SmtpServer = "smtp.gmail.com";
        private const string SmtpUsername = "msl2430@gmail.com";
        private const string SmtpPassword = "1ftm2fts";
        private const int PortNumber = 587;

        public static void SendEmail(MailMessage email)
        {
            //these headers help spam filters allow our send-as-user feature to avoid spam boxes            
			email.ReplyToList.Add(email.From);

            email.Headers["Resent-From"] = SmtpUsername;

            email.Headers["Resent-Date"] = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss zzz");

            using (var mailClient = new SmtpClient(SmtpServer, PortNumber))
			{
				mailClient.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
                mailClient.EnableSsl = true;
                
				mailClient.Send(email);
			}
        }
    }
}