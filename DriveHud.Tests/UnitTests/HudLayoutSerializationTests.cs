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
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model.Enums;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
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
        /// Method tests if <see cref="HudLayoutInfoV2"/> can be serialized/deserialized with <seealso cref="XmlSerializer"/>
        /// </summary>
        [Test]
        public void HudLayoutInfoV2CanBeSerializedDeserialized()
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
            var expectedTool = hudLayoutExpected.LayoutTools.OfType<HudLayoutPlainBoxTool>().First();

            Assert.That(actualTool.Id, Is.EqualTo(expectedTool.Id));

            var actualPositionInfo = actualTool.Positions.FirstOrDefault();
            var expectedPositionInfo = expectedTool.Positions.FirstOrDefault();

            Assert.IsNotNull(actualPositionInfo);
            Assert.IsNotNull(expectedPositionInfo);
            Assert.IsNotNull(actualPositionInfo.HudPositions);
            Assert.IsNotNull(expectedPositionInfo.HudPositions);

            Assert.That(actualPositionInfo.GameType, Is.EqualTo(expectedPositionInfo.GameType));

            var actualPosition = actualPositionInfo.HudPositions.FirstOrDefault();
            var expectedPosition = expectedPositionInfo.HudPositions.FirstOrDefault();

            Assert.That(actualPosition.Position, Is.EqualTo(actualPosition.Position));
            Assert.That(actualPosition.Width, Is.EqualTo(actualPosition.Width));
            Assert.That(actualPosition.Seat, Is.EqualTo(actualPosition.Seat));
            Assert.That(actualPosition.Height, Is.EqualTo(actualPosition.Height));
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
        /// Method tests if <see cref="HudPlainStatBoxViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudPlainStatBoxViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutPlainBoxTool
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

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudPlainStatBoxViewModel;

            Assert.IsNotNull(hudToolViewModelExpected);

            byte[] data = null;

            Assert.DoesNotThrow(() =>
            {
                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, hudToolViewModelExpected);
                    data = msTestString.ToArray();
                }
            });

            Assert.IsNotNull(data);

            HudPlainStatBoxViewModel hudToolViewModelActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudToolViewModelActual = Serializer.Deserialize<HudPlainStatBoxViewModel>(afterStream);
                }
            });

            Assert.IsNotNull(hudToolViewModelActual);
            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Stats.Count, Is.EqualTo(hudToolViewModelExpected.Stats.Count));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutGaugeIndicator"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutGaugeIndicatorCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.CBet },
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP }
                },
                Text = "Test",
                IsVertical = true
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

            HudLayoutGaugeIndicator hudLayoutToolActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudLayoutToolActual = Serializer.Deserialize<HudLayoutGaugeIndicator>(afterStream);
                }
            });

            Assert.IsNotNull(hudLayoutToolActual);
            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.Text, Is.EqualTo(hudLayoutToolExpected.Text));
            Assert.That(hudLayoutToolActual.IsVertical, Is.EqualTo(hudLayoutToolExpected.IsVertical));
            Assert.That(hudLayoutToolActual.BaseStat.Stat, Is.EqualTo(hudLayoutToolExpected.BaseStat.Stat));
            Assert.That(hudLayoutToolActual.Stats.Count, Is.EqualTo(hudLayoutToolExpected.Stats.Count));
            Assert.That(hudLayoutToolActual.Stats.FirstOrDefault().Stat, Is.EqualTo(hudLayoutToolExpected.Stats.FirstOrDefault().Stat));
        }

        /// <summary>
        /// Method tests if <see cref="HudGaugeIndicatorViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudGaugeIndicatorViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.CBet },
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP },
                    new StatInfo { Stat = Stat.PFR },
                    new StatInfo { Stat = Stat.S3Bet },
                    new StatInfo { Stat = Stat.ColdCall }
                },
                Text = "test",
                IsVertical = true
            };

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudGaugeIndicatorViewModel;

            Assert.IsNotNull(hudToolViewModelExpected);

            byte[] data = null;

            Assert.DoesNotThrow(() =>
            {
                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, hudToolViewModelExpected);
                    data = msTestString.ToArray();
                }
            });

            Assert.IsNotNull(data);

            HudGaugeIndicatorViewModel hudToolViewModelActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudToolViewModelActual = Serializer.Deserialize<HudGaugeIndicatorViewModel>(afterStream);
                }
            });

            Assert.IsNotNull(hudToolViewModelActual);
            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Stats.Count, Is.EqualTo(hudToolViewModelExpected.Stats.Count));
            Assert.That(hudToolViewModelActual.BaseStat.Stat, Is.EqualTo(hudToolViewModelExpected.BaseStat.Stat));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Text, Is.EqualTo(hudToolViewModelExpected.Text));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
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
        public void HudLayoutFourStatsBoxViewModelToolCanBeSerializedDeserializedWithProtobuf()
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
        /// Method tests if <see cref="HudLayoutTiltMeterTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudTiltMeterViewModelCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutTiltMeterTool();

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

            HudLayoutTiltMeterTool hudLayoutToolActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudLayoutToolActual = Serializer.Deserialize<HudLayoutTiltMeterTool>(afterStream);
                }
            });

            Assert.IsNotNull(hudLayoutToolActual);
            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
        }

        /// <summary>
        /// Method tests if <see cref="HudTiltMeterViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudTitlMeterViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutTiltMeterTool();

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudTiltMeterViewModel;

            Assert.IsNotNull(hudToolViewModelExpected);

            byte[] data = null;

            Assert.DoesNotThrow(() =>
            {
                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, hudToolViewModelExpected);
                    data = msTestString.ToArray();
                }
            });

            Assert.IsNotNull(data);

            HudTiltMeterViewModel hudToolViewModelActual = null;

            Assert.DoesNotThrow(() =>
            {
                using (var afterStream = new MemoryStream(data))
                {
                    hudToolViewModelActual = Serializer.Deserialize<HudTiltMeterViewModel>(afterStream);
                }
            });

            Assert.IsNotNull(hudToolViewModelActual);
            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
        }

        /// <summary>
        /// Method tests if <see cref="Color"/> can be serialized/deserialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void ColorCanBeSerializedDeserializedWithProtobuf()
        {
            var color = Color.FromArgb(255, 255, 0, 0);

            byte[] serializedColor;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, color);
                serializedColor = ms.ToArray();
            }

            Color actualColor;

            using (var ms = new MemoryStream(serializedColor))
            {
                actualColor = Serializer.Deserialize<Color>(ms);
            }

            Assert.That(actualColor, Is.EqualTo(color));
        }

        /// <summary>
        /// Method tests if <see cref="StatInfoMeterModel"/> can be serialized/deserialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void StatInfoMeterModelCanBeSerializedDeserializedWithProtobuf()
        {
            var statInfoMeterModel = new StatInfoMeterModel();

            statInfoMeterModel.UpdateBackgroundBrushes("#FF28F0DD", "#FF28C3F0", "#FF289EF0", "#FF2868F0", "#FF283AF0", "#FF3812E4", "#FF3812E4", "#FF7B12E4", "#FFD112E4", "#FFE412A1");
            statInfoMeterModel.UpdateBorderBrushes("#FF59FDED", "#FF48D9F0", "#FF48ABF0", "#FF4885F0", "#FF4857F0", "#FF5B2DF5", "#FF5B2DF5", "#FF882DF5", "#FFDA2DF5", "#FFF52DD1");

            byte[] serializedStatInfoMeterModel;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, statInfoMeterModel);
                serializedStatInfoMeterModel = ms.ToArray();
            }

            StatInfoMeterModel actualStatInfoMeterModel;

            using (var ms = new MemoryStream(serializedStatInfoMeterModel))
            {
                actualStatInfoMeterModel = Serializer.Deserialize<StatInfoMeterModel>(ms);
            }

            Assert.That(actualStatInfoMeterModel.BackgroundBrush.Length, Is.EqualTo(statInfoMeterModel.BackgroundBrush.Length));
            Assert.That(actualStatInfoMeterModel.BorderBrush.Length, Is.EqualTo(statInfoMeterModel.BorderBrush.Length));
            Assert.That(actualStatInfoMeterModel.BorderBrush.Length, Is.EqualTo(statInfoMeterModel.BackgroundBrush.Length));

            for (var i = 0; i < statInfoMeterModel.BackgroundBrush.Length; i++)
            {
                Assert.That(actualStatInfoMeterModel.BackgroundBrush[i].Color, Is.EqualTo(statInfoMeterModel.BackgroundBrush[i].Color));
                Assert.That(actualStatInfoMeterModel.BorderBrush[i].Color, Is.EqualTo(statInfoMeterModel.BorderBrush[i].Color));
            }
        }

        /// <summary>
        /// Creates <see cref="HudLayoutInfoV2"/> for tests, all tools must be included
        /// </summary>
        /// <returns><see cref="HudLayoutInfoV2"/></returns>
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

            var tiltMeterTool = new HudLayoutTiltMeterTool();

            hudLayoutInfo.LayoutTools = new List<HudLayoutTool> { plainBoxTool, textboxTool, fourStatBoxTool, tiltMeterTool };

            return hudLayoutInfo;
        }
    }
}