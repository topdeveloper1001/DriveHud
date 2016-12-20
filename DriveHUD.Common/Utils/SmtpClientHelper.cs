using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public static MailMessage ComposeSupportEmail(string userName, string userEmail, string userMessage,
            string appFolder, string dataFolder)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(userEmail);
            mail.To.Add(_supportEmail);

            mail.Subject = "DriveHUD message from user " + userName;
            mail.Body = string.Format(_supportMessageBody, userName, userMessage);

            if (!string.IsNullOrEmpty(appFolder))
            {
                AddZipAtachment(mail,
                    Directory.GetFiles(appFolder)
                        .Where(
                            f =>
                                string.Equals(Path.GetExtension(f), ".config",
                                    StringComparison.InvariantCultureIgnoreCase)), "Configs.zip");
                var logsFolder = Path.Combine(appFolder, "Logs");
                AddZipAtachment(mail, Directory.GetFiles(logsFolder), "Logs.zip");
            }
            if (!string.IsNullOrEmpty(dataFolder))
            {
                var extensionsToZip = new[] {".data", ".xml", ".df"};
                AddZipAtachment(mail,
                    Directory.GetFiles(dataFolder)
                        .Where(
                            f =>
                                extensionsToZip.Contains(Path.GetExtension(f),
                                    StringComparer.InvariantCultureIgnoreCase)), "Data.zip");
            }
            return mail;
        }

        //public static MailMessage ComposeSupportLogsEmail()
        //{
        //    MailMessage mail = new MailMessage();

        //    mail.From = new MailAddress(_supportEmail);
        //    mail.To.Add(_supportEmail);

        //    mail.Subject = "DriveHUD logs from user";
        //    mail.Body = "Log files from user.";

        //    AddAttachments(mail);

        //    return mail;
        //}

        private static void AddZipAtachment(MailMessage mail, IEnumerable<string> files, string zipFileName)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            using (var entryStream = archive.CreateEntry(Path.GetFileName(file)).Open())
                            {
                                using (var streamWriter = new StreamWriter(entryStream))
                                {
                                    using (var s = new FileStream(file, FileMode.Open, FileAccess.Read))
                                    {
                                        streamWriter.Write(s);
                                    }
                                }
                            }
                        }
                    }
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    mail.Attachments.Add(new Attachment(memoryStream, Path.GetFileName(zipFileName)));
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }
        }
    }
}
