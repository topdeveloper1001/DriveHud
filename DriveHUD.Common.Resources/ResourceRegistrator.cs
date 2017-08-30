using System.Reflection;
using System.Threading;

namespace DriveHUD.Common.Resources
{
    public class ResourceRegistrator
    {
        public static void Initialization()
        {
            RegisterResources(CommonResourceManager.Instance);
        }

        static int resourcesRegistered;
        static readonly FileResourceManager commonResourceManager = new FileResourceManager("Common_", "DriveHUD.Common.Resources.Common", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager errorsResourceManager = new FileResourceManager("Error_", "DriveHUD.Common.Resources.Errors", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager enumsResourceManager = new FileResourceManager("Enum_", "DriveHUD.Common.Resources.Enums", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager progressMessagesResourceManager = new FileResourceManager("Progress_", "DriveHUD.Common.Resources.ProgressMessages", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager messagesResourceManager = new FileResourceManager("Message_", "DriveHUD.Common.Resources.Messages", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager mainResourceManager = new FileResourceManager("Main_", "DriveHUD.Common.Resources.Main", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager showdownHandsResourceManager = new FileResourceManager("Showdown_", "DriveHUD.Common.Resources.ShowdownHands", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager settingsResourceManager = new FileResourceManager("Settings_", "DriveHUD.Common.Resources.Settings", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager systemSettingsResourceManager = new FileResourceManager("SystemSettings_", "DriveHUD.Common.Resources.SystemSettings", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager notificationsResourceManager = new FileResourceManager("Notifications_", "DriveHUD.Common.Resources.Notifications", Assembly.GetExecutingAssembly());
        static readonly FileResourceManager reportsResourceManager = new FileResourceManager("Reports_", "DriveHUD.Common.Resources.Reports", Assembly.GetExecutingAssembly());

        public static void RegisterResources(CommonResourceManager resourceManager)
        {
            if (Interlocked.Exchange(ref resourcesRegistered, 1) == 1)
            {
                return;
            }

            resourceManager.RegisterResourceManager(commonResourceManager);
            resourceManager.RegisterResourceManager(errorsResourceManager);
            resourceManager.RegisterResourceManager(enumsResourceManager);
            resourceManager.RegisterResourceManager(progressMessagesResourceManager);
            resourceManager.RegisterResourceManager(messagesResourceManager);
            resourceManager.RegisterResourceManager(mainResourceManager);
            resourceManager.RegisterResourceManager(showdownHandsResourceManager);
            resourceManager.RegisterResourceManager(settingsResourceManager);
            resourceManager.RegisterResourceManager(systemSettingsResourceManager);
            resourceManager.RegisterResourceManager(notificationsResourceManager);
            resourceManager.RegisterResourceManager(reportsResourceManager);
        }
    }
}