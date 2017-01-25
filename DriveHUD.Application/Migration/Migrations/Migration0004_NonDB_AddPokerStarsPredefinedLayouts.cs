using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(4)]
    public class Migration0004_NonDB_AddPokerStarsPredefinedLayouts : Migration
    {
        private string[] predefinedLayouts = new string[] {
        "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-MTT-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-10max-SNG-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-MTT-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-2max-SNG-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-MTT-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-4max-SNG-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-MTT-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-6max-SNG-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-MTT-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-8max-SNG-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-9max-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-FR-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-MTT-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-MTT-Basic-PS.xml",
            "DriveHUD.Common.Resources.Layouts.DH-SNG-Basic-OH-PS.xml", "DriveHUD.Common.Resources.Layouts.DH-SNG-Basic-PS.xml"
        };

        public override void Up()
        {
            LogProvider.Log.Info("Running migration #4");
            try
            {
                //LoadPredefinedLayoutsForPS();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }

            LogProvider.Log.Info("Migration #4 executed.");
        }

        //private void LoadPredefinedLayoutsForPS()
        //{
        //    var layoutsService = new ViewModels.Hud.HudLayoutsService();

        //    if (!layoutsService.Layouts.Any())
        //    {
        //        return;
        //    }

        //    var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

        //    var path = "tempfile.layouts";
        //    foreach (var predefinedLayout in predefinedLayouts)
        //    {
        //        using (var stream = resourcesAssembly.GetManifestResourceStream(predefinedLayout))
        //        {
        //            using (var f = File.Create(path))
        //            {
        //                stream.CopyTo(f);
        //            }

        //            layoutsService.Import(path);

        //            try
        //            {
        //                stream.Seek(0, SeekOrigin.Begin);

        //                var xmlSerializer = new XmlSerializer(typeof(HudSavedLayouts));
        //                var importedHudLayouts = xmlSerializer.Deserialize(stream) as HudSavedLayouts;

        //                var importedHudLayout = importedHudLayouts.Layouts.FirstOrDefault();

        //                if (importedHudLayout != null)
        //                {
        //                    layoutsService.SetLayoutActive(importedHudLayout);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                LogProvider.Log.Error(this, e);
        //            }
        //        }
        //    }

        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //    }
        //}

        public override void Down()
        {
        }
    }
}
