using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(9)]
    public class Migration0009_NonDB_AddAmericasCardroomPredefinedLayouts : Migration
    {
        private string[] predefinedLayouts = new string[] {
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-3max-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-4max-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-8max-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-9max-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-MTT-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-9max-MTT-Basic-OH-ACR.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-SNG-Basic-ACR.xml","DriveHUD.Common.Resources.Layouts.DH-9max-SNG-Basic-OH-ACR.xml"
        };

        public override void Up()
        {
            LogProvider.Log.Info("Running migration #9");
            try
            {
                //LoadPredefinedLayoutsForACR();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            LogProvider.Log.Info("Migration #9 executed.");
        }

        private void LoadPredefinedLayoutsForACR()
        {
            var layoutsService = new ViewModels.Hud.HudLayoutsService();

            if (!layoutsService.Layouts.Any())
            {
                return;
            }

            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            var path = "tempfile.layouts";

            foreach (var predefinedLayout in predefinedLayouts)
            {
                using (var stream = resourcesAssembly.GetManifestResourceStream(predefinedLayout))
                {
                    using (var f = File.Create(path))
                    {
                        stream.CopyTo(f);
                    }

                    layoutsService.Import(path);

                    try
                    {
                        stream.Seek(0, SeekOrigin.Begin);

                        var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));
                        var importedHudLayouts = xmlSerializer.Deserialize(stream) as HudSavedLayouts;

                        var importedHudLayout = importedHudLayouts.Layouts.FirstOrDefault();

                        if (importedHudLayout != null)
                        {
                            layoutsService.SetLayoutActive(importedHudLayout);
                        }
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, e);
                    }
                }
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public override void Down()
        {
        }
    }
}
