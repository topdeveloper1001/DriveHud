//-----------------------------------------------------------------------
// <copyright file="HudLayoutInfoTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model.Enums;
using NUnit.Framework;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class HudLayoutInfoTests
    {
        /// <summary>
        /// Method tests if <see cref="HudLayoutInfoV2"/> can be serialized with <seealso cref="XmlSerializer"/>
        /// </summary>
        [Test]
        public void HudLayoutInfoCanBeSerialized()
        {
            var hudLayoutInfo = CreateHudLayoutInfo();

            Assert.DoesNotThrow(() =>
            {
                using (var sw = new StringWriter())
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                    xmlSerializer.Serialize(sw, hudLayoutInfo);

                    Debug.WriteLine(sw.ToString());
                }
            });
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutPlainBoxTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutPlainBoxTool();

            byte[] data = null;

            Assert.DoesNotThrow(() =>
            {
                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, hudLayoutToolExpected);
                    data = msTestString.ToArray();
                }
            });

            Assert.IsNotNull(data);

            HudLayoutPlainBoxTool hudLayoutToolActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudLayoutToolActual = Serializer.Deserialize<HudLayoutPlainBoxTool>(afterStream);
                }
            });

            Assert.IsNotNull(hudLayoutToolActual);
            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutPlainBoxTool"/> can be serialized with <seealso cref="XmlSerializer"/>
        /// </summary>
        [Test]
        public void HudLayoutToolCanBeSerializedDeserialized()
        {
            var hudLayoutExpected = CreateHudLayoutInfo();

            string serialized = null;

            Assert.DoesNotThrow(() =>
            {
                using (var sw = new StringWriter())
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                    xmlSerializer.Serialize(sw, hudLayoutExpected);
                    serialized = sw.ToString();
                }
            });

            Assert.IsNotNull(serialized);

            HudLayoutInfoV2 hudLayoutActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var sr = new StringReader(serialized))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                    hudLayoutActual = xmlSerializer.Deserialize(sr) as HudLayoutInfoV2;
                }
            });

            Assert.IsNotNull(hudLayoutActual);

            var actualTool = hudLayoutActual.LayoutTools.OfType<HudLayoutPlainBoxTool>().First();
            var expectedlTool = hudLayoutExpected.LayoutTools.OfType<HudLayoutPlainBoxTool>().First();

            Assert.That(actualTool.Id, Is.EqualTo(expectedlTool.Id));
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

        private static HudLayoutInfoV2 CreateHudLayoutInfo()
        {
            var hudLayoutInfo = new HudLayoutInfoV2
            {
                TableType = EnumTableType.HU,
                Name = "TestLayout"
            };

            var plainBoxTool = new HudLayoutPlainBoxTool
            {
                Positions = new List<HudPositionsInfo>
                {
                      new HudPositionsInfo
                      {
                           GameType = EnumGameType.CashHoldem,
                           HudPositions = new List<HudPositionInfo>
                           {
                                new HudPositionInfo
                                {
                                     Position = new System.Windows.Point(1,1),
                                     Seat = 1,
                                     Width = 2
                                }
                           },
                           PokerSite = EnumPokerSites.Bodog
                      }
                },
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo
                    {
                        Stat = Stat.VPIP
                    }
                }
            };

            var textboxTool = new HudLayoutTextBoxTool
            {
                Text = "Test"
            };

            var fourStatBoxTool = new HudLayoutFourStatsBoxTool
            {
                Positions = new List<HudPositionsInfo>
                {
                      new HudPositionsInfo
                      {
                           GameType = EnumGameType.CashHoldem,
                           HudPositions = new List<HudPositionInfo>
                           {
                                new HudPositionInfo
                                {
                                     Position = new System.Windows.Point(1,1),
                                     Seat = 1,
                                     Width = 2
                                }
                           },
                           PokerSite = EnumPokerSites.Bodog
                      }
                },
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo
                    {
                        Stat = Stat.VPIP
                    }
                }
            };

            hudLayoutInfo.LayoutTools = new List<HudLayoutTool> { plainBoxTool, textboxTool, fourStatBoxTool };

            return hudLayoutInfo;
        }
    }
}