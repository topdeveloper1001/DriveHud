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
    [Migration(6)]
    public class Migration0006_NonDB_Add888PokerPredefinedLayouts : Migration
    {
        private string[] predefinedLayouts = new string[] {
            "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-10max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-3max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-3max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-3max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-4max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-5max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-5max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-5max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-5max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-5max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-5max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-8max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-9max-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-MTT-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-9max-MTT-Basic-OH-888.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-SNG-Basic-888.xml","DriveHUD.Common.Resources.Layouts.DH-9max-SNG-Basic-OH-888.xml"
        };

        public override void Up()
        {
            LogProvider.Log.Info("Running migration #6");
            try
            {
                //LoadPredefinedLayoutsForPS();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            LogProvider.Log.Info("Migration #6 executed.");
        }

        private void LoadPredefinedLayoutsForPS()
        {
            var layoutsService = new ViewModels.Hud.HudLayoutsService();

            if (!layoutsService.Layouts.Layouts.Any())
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
