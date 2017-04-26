//-----------------------------------------------------------------------
// <copyright file="HudLayoutMigrationTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Surrogates;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model.Enums;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DriveHud.Tests.UnitTests
{
    class HudLayoutMigrationTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.Color), false).SetSurrogate(typeof(ColorDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Point), false).SetSurrogate(typeof(PointDto));
        }

        /// <summary>
        /// temp test
        /// </summary>
        //[Test]
        public void MigrateDefaultLayouts()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            ResourceRegistrator.Initialization();

            foreach (var tableType in Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>())
            {
                MigrateDefaultLayout(tableType);
            }
        }

        private void MigrateDefaultLayout(EnumTableType tableType)
        {
            var layout = GetDefaultLayout(tableType);
            var newLayout = ConvertLayout(layout);
            SaveLayout(newLayout);
        }

        private HudLayoutInfoV2 ConvertLayout(HudLayoutInfo layout)
        {
            var result = new HudLayoutInfoV2
            {
                Name = layout.Name,
                IsDefault = layout.IsDefault,
                Opacity = layout.HudOpacity,
                TableType = layout.TableType,
                HudBumperStickerTypes = layout.HudBumperStickerTypes,
                HudPlayerTypes = layout.HudPlayerTypes,
                LayoutTools = new List<HudLayoutTool>
                {
                    new HudLayoutPlainBoxTool
                    {
                        Stats = new ReactiveList<StatInfo>(layout.HudStats),
                        Positions = layout.HudPositionsInfo,
                        UIPositions = layout.UiPositionsInfo.Select(x=>
                            new HudPositionInfo
                            {
                                Position = x.Position,
                                Seat = x.Seat
                            }).ToList()
                    }
                }
            };

            return result;
        }

        private HudLayoutInfo GetDefaultLayout(EnumTableType tableType)
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            var path = $"DriveHUD.Common.Resources.Layouts.Default-{CommonResourceManager.Instance.GetEnumResource(tableType)}.xml";

            using (var stream = resourcesAssembly.GetManifestResourceStream(path))
            {
                return LoadLayoutFromStream(stream);
            }
        }

        private HudLayoutInfo LoadLayoutFromStream(Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
            var layout = xmlSerializer.Deserialize(stream) as HudLayoutInfo;
            return layout;
        }

        private void SaveLayout(HudLayoutInfoV2 layout)
        {
            var fileName = ($"DH: {CommonResourceManager.Instance.GetEnumResource(layout.TableType)}.xml").RemoveInvalidFileNameChars();

            using (var fs = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                xmlSerializer.Serialize(fs, layout);
            }
        }
    }
}