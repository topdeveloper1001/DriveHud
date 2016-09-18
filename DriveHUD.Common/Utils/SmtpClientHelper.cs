using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Utils
{
    public static class SmtpClientHelper
    {
        private static readonly string _supportEmail = CommonResourceManager.GetResourceStringForCompositeKey("SystemSettings_SupportEmail");
        private static readonly string _supportPassword = @"DHmexico121@";
        private static readonly string _supportMessageBody = CommonResourceManager.GetResourceStringForCompositeKey("SystemSettings_SupportMessageBody");

        public static SmtpClient GetSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient("mail.drivehud.com", 26);

            smtpClient.EnableSsl = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(_supportEmail, _supportPassword);

            return smtpClient;
        }

        public static MailMessage ComposeSupportEmail(string userName, string userEmail, string userMessage, bool attachLogs = false)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(userEmail);
            mail.To.Add(_supportEmail);

            mail.Subject = "DriveHUD message from user " + userName;
            mail.Body = String.Format(_supportMessageBody, userName, userMessage);

            if (attachLogs)
            {
                AddAttachments(mail);
            }

            return mail;
        }

        public static MailMessage ComposeSupportLogsEmail()
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(_supportEmail);
            mail.To.Add(_supportEmail);

            mail.Subject = "DriveHUD logs from user";
            mail.Body = "Log files from user.";

            AddAttachments(mail);

            return mail;
        }

        private static void AddAttachments(MailMessage mail)
        {
            try
            {
                var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (Directory.Exists(logFolder))
                {
                    foreach (var file in Directory.GetFiles(logFolder))
                    {
                        if (Path.GetFileName(file) == "drivehud.log")
                        {
                            Stream s = new FileStream(file, FileMode.Open, FileAccess.Read);
                            mail.Attachments.Add(new Attachment(s, Path.GetFileName(file)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }
    }
}
