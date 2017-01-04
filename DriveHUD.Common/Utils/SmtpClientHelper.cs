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

        private static readonly string[] _advancedSupportEmails = new string[]
        {
            "aleksander.v.danilov@gmail.com",
            "john@acepokersolutions.com"
        };

        private static readonly string _supportPassword = @"DHmexico121@";

        private static readonly string _supportMessageBody =
            CommonResourceManager.GetResourceStringForCompositeKey("SystemSettings_SupportMessageBody");

        public static SmtpClient GetSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient("mail.drivehud.com", 26);

            smtpClient.EnableSsl = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(_supportEmail, _supportPassword);
            smtpClient.SendCompleted += (o, e) =>
            {
                var message = e.UserState as MailMessage;
                if (message == null)
                    return;
                foreach (var messageAttachment in message.Attachments)
                {
                    messageAttachment.Dispose();
                }
            };
            return smtpClient;
        }

        public static MailMessage ComposeSupportEmail(string userName, string userEmail, string userMessage,
            string appFolder, string dataFolder, bool sendAdvancedLog = false)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(userEmail);

            if (sendAdvancedLog)
            {
                foreach (var email in _advancedSupportEmails)
                {
                    mail.To.Add(email);
                }
            }
            else
            {
                mail.To.Add(_supportEmail);
            }

            mail.Subject = "DriveHUD message from user " + userName;
            mail.Body = string.Format(_supportMessageBody, userName, userMessage);

            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                if (sendAdvancedLog)
                {
                    AddAdvancedLog(appFolder, dataFolder, archive);
                }
                else
                {
                    AddSimpleLog(appFolder, archive);
                }
            }
            if (memoryStream.Length <= 0)
                return mail;
            memoryStream.Seek(0, SeekOrigin.Begin);
            mail.Attachments.Add(new Attachment(memoryStream, Path.GetFileName("drivehud.zip")));
            return mail;
        }

        private static void AddSimpleLog(string appFolder, ZipArchive archive)
        {
            if (!string.IsNullOrWhiteSpace(appFolder))
            {
                var logs = new string[]
                {
                    Path.Combine(appFolder, "Logs", "drivehud.log"),
                    Path.Combine(appFolder, "Logs", "hud.log")
                }.Where(x => File.Exists(x));

                if (logs.Any())
                {
                    AddZipAtachment(archive, logs);
                }
            }
        }

        private static void AddAdvancedLog(string appFolder, string dataFolder, ZipArchive archive)
        {
            if (!string.IsNullOrEmpty(appFolder))
            {
                AddZipAtachment(archive,
                    Directory.GetFiles(appFolder)
                        .Where(f => string.Equals(Path.GetExtension(f), ".config", StringComparison.InvariantCultureIgnoreCase)));
                var logsFolder = Path.Combine(appFolder, "Logs");
                AddZipAtachment(archive, Directory.GetFiles(logsFolder), "Logs");
            }

            if (!string.IsNullOrEmpty(dataFolder))
            {
                var extensionsToZip = new[] { ".data", ".xml", ".df" };
                AddZipAtachment(archive,
                    Directory.GetFiles(dataFolder)
                        .Where(
                            f =>
                                extensionsToZip.Contains(Path.GetExtension(f),
                                    StringComparer.InvariantCultureIgnoreCase)));
            }
        }

        private static void AddZipAtachment(ZipArchive archive, IEnumerable<string> files, string subFolderName = null)
        {
            try
            {
                foreach (var file in files)
                {
                    try
                    {
                        var entryName = string.IsNullOrEmpty(subFolderName)
                            ? Path.GetFileName(file)
                            : Path.Combine(subFolderName, Path.GetFileName(file));
                        using (var entryStream = archive.CreateEntry(entryName).Open())
                        {
                            using (var s = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                s.CopyTo(entryStream);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        LogProvider.Log.Error(ex);
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
