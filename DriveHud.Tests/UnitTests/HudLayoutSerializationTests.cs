//-----------------------------------------------------------------------
// <copyright file="HudLayoutSerializationTests.cs" company="Ace Poker Solutions">
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
using System.Xml.Serialization;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class HudLayoutSerializationTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.Color), false).SetSurrogate(typeof(ColorDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Point), false).SetSurrogate(typeof(PointDto));
        }

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
        public void HudLayoutPlainBoxToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutPlainBoxTool
            {
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP }
                }
            };            

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
            Assert.That(hudLayoutToolActual.Stats.Count, Is.EqualTo(hudLayoutToolExpected.Stats.Count));
            Assert.That(hudLayoutToolActual.Stats.FirstOrDefault().Stat, Is.EqualTo(hudLayoutToolExpected.Stats.FirstOrDefault().Stat));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutFourStatsBoxTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutFourStatsBoxToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutFourStatsBoxTool
            {
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP },
                    new StatInfo { Stat = Stat.PFR },
                    new StatInfo { Stat = Stat.S3Bet },
                    new StatInfo { Stat = Stat.ColdCall }
                }
            };

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

            HudLayoutFourStatsBoxTool hudLayoutToolActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudLayoutToolActual = Serializer.Deserialize<HudLayoutFourStatsBoxTool>(afterStream);
                }
            });

            Assert.IsNotNull(hudLayoutToolActual);
            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
            Assert.That(hudLayoutToolActual.Stats.Count, Is.EqualTo(hudLayoutToolExpected.Stats.Count));
        }

        /// <summary>
        /// Method tests if <see cref="HudFourStatsBoxViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudFourStatsBoxVewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutFourStatsBoxTool
            {
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP },
                    new StatInfo { Stat = Stat.PFR },
                    new StatInfo { Stat = Stat.S3Bet },
                    new StatInfo { Stat = Stat.ColdCall }
                }
            };

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudFourStatsBoxViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudFourStatsBoxViewModel;

            Assert.IsNotNull(hudFourStatsBoxViewModelExpected);

            byte[] data = null;

            Assert.DoesNotThrow(() =>
            {
                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, hudFourStatsBoxViewModelExpected);
                    data = msTestString.ToArray();
                }
            });

            Assert.IsNotNull(data);

            HudFourStatsBoxViewModel hudFourStatsBoxViewModelActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudFourStatsBoxViewModelActual = Serializer.Deserialize<HudFourStatsBoxViewModel>(afterStream);
                }
            });

            Assert.IsNotNull(hudFourStatsBoxViewModelActual);
            Assert.That(hudFourStatsBoxViewModelActual.Id, Is.EqualTo(hudFourStatsBoxViewModelExpected.Id));
            Assert.That(hudFourStatsBoxViewModelActual.ToolType, Is.EqualTo(hudFourStatsBoxViewModelExpected.ToolType));
            Assert.That(hudFourStatsBoxViewModelActual.Stats.Count, Is.EqualTo(hudFourStatsBoxViewModelExpected.Stats.Count));
            Assert.That(hudFourStatsBoxViewModelActual.Stat1.Stat, Is.EqualTo(hudFourStatsBoxViewModelExpected.Stat1.Stat));
            Assert.That(hudFourStatsBoxViewModelActual.Stat2.Stat, Is.EqualTo(hudFourStatsBoxViewModelExpected.Stat2.Stat));
            Assert.That(hudFourStatsBoxViewModelActual.Stat3.Stat, Is.EqualTo(hudFourStatsBoxViewModelExpected.Stat3.Stat));
            Assert.That(hudFourStatsBoxViewModelActual.Stat4.Stat, Is.EqualTo(hudFourStatsBoxViewModelExpected.Stat4.Stat));
            Assert.That(hudFourStatsBoxViewModelActual.Width, Is.EqualTo(hudFourStatsBoxViewModelExpected.Width));
            Assert.That(hudFourStatsBoxViewModelActual.Height, Is.EqualTo(hudFourStatsBoxViewModelExpected.Height));
            Assert.That(hudFourStatsBoxViewModelActual.Position, Is.EqualTo(hudFourStatsBoxViewModelExpected.Position));
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