﻿//-----------------------------------------------------------------------
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

using DriveHud.Tests.UnitTests.Helpers;
using DriveHUD.Application.Surrogates;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;
using Model.Data;
using Model.Enums;
using Model.Stats;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using ReactiveUI;
using System;
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
            try
            {
                RuntimeTypeModel.Default.Add(typeof(Color), false).SetSurrogate(typeof(ColorDto));
                RuntimeTypeModel.Default.Add(typeof(SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
                RuntimeTypeModel.Default.Add(typeof(System.Windows.Point), false).SetSurrogate(typeof(PointDto));
            }
            // ignore InvalidOperationException, because serializer might be generated in other test
            catch (InvalidOperationException)
            {
            }
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
            Assert.That(hudLayoutActual.Filter, Is.EqualTo(hudLayoutExpected.Filter));

            Assert.That(hudLayoutActual.TrackMeterPositions.Count, Is.EqualTo(hudLayoutExpected.TrackMeterPositions.Count));

            for (var i = 0; i < hudLayoutActual.TrackMeterPositions.Count; i++)
            {
                var actualTrackMeter = hudLayoutActual.TrackMeterPositions[i];
                var expectedTrackMeter = hudLayoutExpected.TrackMeterPositions[i];

                Assert.That(actualTrackMeter.GameType, Is.EqualTo(expectedTrackMeter.GameType));
                Assert.That(actualTrackMeter.PokerSite, Is.EqualTo(expectedTrackMeter.PokerSite));
                Assert.That(actualTrackMeter.HudPositions.Count, Is.EqualTo(expectedTrackMeter.HudPositions.Count));
            }
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

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

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
            hudToolViewModelExpected.Width = 100;
            hudToolViewModelExpected.Height = 150;
            hudToolViewModelExpected.Position = new System.Windows.Point(10, 10);
            hudToolViewModelExpected.Opacity = 50;
            hudToolViewModelExpected.IsNoteIconEnabled = true;

            Assert.IsNotNull(hudToolViewModelExpected);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Stats.Count, Is.EqualTo(hudToolViewModelExpected.Stats.Count));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Opacity, Is.EqualTo(hudToolViewModelExpected.Opacity));
            Assert.That(hudToolViewModelActual.IsNoteIconEnabled, Is.EqualTo(hudToolViewModelExpected.IsNoteIconEnabled));
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
                HeaderText = "HeaderText",
                IsVertical = true,
                Tools = new ReactiveList<HudLayoutTool>
                {
                    new HudLayoutHeatMapTool
                    {
                        BaseStat = new StatInfo { Stat = Stat.VPIP }
                    }
                }
            };

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.Text, Is.EqualTo(hudLayoutToolExpected.Text));
            Assert.That(hudLayoutToolActual.HeaderText, Is.EqualTo(hudLayoutToolExpected.HeaderText));
            Assert.That(hudLayoutToolActual.IsVertical, Is.EqualTo(hudLayoutToolExpected.IsVertical));
            Assert.That(hudLayoutToolActual.BaseStat.Stat, Is.EqualTo(hudLayoutToolExpected.BaseStat.Stat));
            Assert.That(hudLayoutToolActual.Stats.Count, Is.EqualTo(hudLayoutToolExpected.Stats.Count));
            Assert.That(hudLayoutToolActual.Stats.FirstOrDefault().Stat, Is.EqualTo(hudLayoutToolExpected.Stats.FirstOrDefault().Stat));
            Assert.That(hudLayoutToolActual.Tools.Count, Is.EqualTo(hudLayoutToolExpected.Tools.Count));
            Assert.That(hudLayoutToolActual.Tools.FirstOrDefault().ToolType, Is.EqualTo(hudLayoutToolExpected.Tools.FirstOrDefault().ToolType));
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
                IsVertical = true,
                HeaderText = "HeaderText",
                Tools = new ReactiveList<HudLayoutTool>
                {
                    new HudLayoutHeatMapTool
                    {
                        BaseStat = new StatInfo { Stat = Stat.PFR}
                    }
                }
            };

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudGaugeIndicatorViewModel;
            hudToolViewModelExpected.IsGraphIndicatorsDisabled = true;

            Assert.IsNotNull(hudToolViewModelExpected.GroupedStats);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Stats.Count, Is.EqualTo(hudToolViewModelExpected.Stats.Count));
            Assert.That(hudToolViewModelActual.BaseStat.Stat, Is.EqualTo(hudToolViewModelExpected.BaseStat.Stat));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Text, Is.EqualTo(hudToolViewModelExpected.Text));
            Assert.That(hudToolViewModelActual.HeaderText, Is.EqualTo(hudToolViewModelExpected.HeaderText));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.IsGraphIndicatorsDisabled, Is.EqualTo(hudToolViewModelExpected.IsGraphIndicatorsDisabled));
            Assert.That(hudToolViewModelActual.GroupedStats.Count, Is.EqualTo(hudToolViewModelExpected.GroupedStats.Count));

            var hudGaugeIndicatorStats = hudToolViewModelActual.GroupedStats.SelectMany(x => x.Stats).ToArray();

            foreach (var groupedStat in hudToolViewModelExpected.GroupedStats)
            {
                foreach (var expectedHudGaugeIndicatorStat in groupedStat.Stats)
                {
                    var actualHudGaugeIndicatorStat = hudGaugeIndicatorStats.FirstOrDefault(x => x.Stat.Stat == expectedHudGaugeIndicatorStat.Stat.Stat);

                    Assert.IsNotNull(actualHudGaugeIndicatorStat);

                    if (expectedHudGaugeIndicatorStat.IsHeatMapVisible)
                    {
                        Assert.That(actualHudGaugeIndicatorStat.IsHeatMapVisible, Is.EqualTo(expectedHudGaugeIndicatorStat.IsHeatMapVisible));
                        Assert.That(actualHudGaugeIndicatorStat.HeatMapViewModel.BaseStat.Stat, Is.EqualTo(expectedHudGaugeIndicatorStat.HeatMapViewModel.BaseStat.Stat));
                    }
                }
            }
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
                },
                IsVertical = true
            };

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
            Assert.That(hudLayoutToolActual.IsVertical, Is.EqualTo(hudLayoutToolExpected.IsVertical));
            Assert.That(hudLayoutToolActual.Stats.Count, Is.EqualTo(hudLayoutToolExpected.Stats.Count));
        }

        /// <summary>
        /// Method tests if <see cref="HudFourStatsBoxViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudFourStatsBoxViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutFourStatsBoxTool
            {
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP },
                    new StatInfo { Stat = Stat.PFR },
                    new StatInfo { Stat = Stat.S3Bet },
                    new StatInfo { Stat = Stat.ColdCall }
                },
                IsVertical = true
            };

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudFourStatsBoxViewModel;
            hudToolViewModelExpected.Width = 100;
            hudToolViewModelExpected.Height = 150;
            hudToolViewModelExpected.Position = new System.Windows.Point(5, 5);
            hudToolViewModelExpected.Opacity = 50;

            Assert.IsNotNull(hudToolViewModelExpected);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Stats.Count, Is.EqualTo(hudToolViewModelExpected.Stats.Count));
            Assert.That(hudToolViewModelActual.Stat1.Stat, Is.EqualTo(hudToolViewModelExpected.Stat1.Stat));
            Assert.That(hudToolViewModelActual.Stat2.Stat, Is.EqualTo(hudToolViewModelExpected.Stat2.Stat));
            Assert.That(hudToolViewModelActual.Stat3.Stat, Is.EqualTo(hudToolViewModelExpected.Stat3.Stat));
            Assert.That(hudToolViewModelActual.Stat4.Stat, Is.EqualTo(hudToolViewModelExpected.Stat4.Stat));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Opacity, Is.EqualTo(hudToolViewModelExpected.Opacity));
            Assert.That(hudToolViewModelActual.IsVertical, Is.EqualTo(hudToolViewModelExpected.IsVertical));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutTiltMeterTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutTiltMeterToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutTiltMeterTool();

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
        }

        /// <summary>
        /// Method tests if <see cref="HudTiltMeterViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudTiltMeterViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutTiltMeterTool();

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudTiltMeterViewModel;
            hudToolViewModelExpected.Width = 100;
            hudToolViewModelExpected.Height = 75;
            hudToolViewModelExpected.Position = new System.Windows.Point(5, 5);
            hudToolViewModelExpected.Opacity = 60;

            Assert.IsNotNull(hudToolViewModelExpected);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Opacity, Is.EqualTo(hudToolViewModelExpected.Opacity));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutPlayerIconTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutPlayerIconToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutPlayerIconTool();

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
        }

        /// <summary>
        /// Method tests if <see cref="HudPlayerIconViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudPlayerIconViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutPlayerIconTool();

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudPlayerIconViewModel;
            hudToolViewModelExpected.Width = 100;
            hudToolViewModelExpected.Height = 75;
            hudToolViewModelExpected.Position = new System.Windows.Point(5, 5);
            hudToolViewModelExpected.Opacity = 60;

            Assert.IsNotNull(hudToolViewModelExpected);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Opacity, Is.EqualTo(hudToolViewModelExpected.Opacity));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutGaugeIndicator"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutGraphToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutGraphTool
            {
                BaseStat = new StatInfo { Stat = Stat.CBet },
                IsVertical = true,
                ParentId = Guid.NewGuid(),
                Stats = new ReactiveList<StatInfo> { new StatInfo { Stat = Stat.AGG } }
            };

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.IsVertical, Is.EqualTo(hudLayoutToolExpected.IsVertical));
            Assert.That(hudLayoutToolActual.BaseStat.Stat, Is.EqualTo(hudLayoutToolExpected.BaseStat.Stat));
            Assert.That(hudLayoutToolActual.ParentId, Is.EqualTo(hudLayoutToolExpected.ParentId));
            Assert.That(hudLayoutToolActual.Stats.Count, Is.EqualTo(hudLayoutToolExpected.Stats.Count));
            Assert.That(hudLayoutToolActual.Stats.First().Stat, Is.EqualTo(hudLayoutToolExpected.Stats.First().Stat));
        }

        /// <summary>
        /// Method tests if <see cref="HudGaugeIndicatorViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudGraphViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutGraphTool
            {
                BaseStat = new StatInfo { Stat = Stat.CBet },
                IsVertical = true,
                Stats = new ReactiveList<StatInfo> { new StatInfo { Stat = Stat.AGG } },
                ParentId = Guid.NewGuid()
            };

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudGraphViewModel;

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.BaseStat.Stat, Is.EqualTo(hudToolViewModelExpected.BaseStat.Stat));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Stats.Count, Is.EqualTo(hudToolViewModelExpected.Stats.Count));
            Assert.That(hudToolViewModelActual.Stats.First().Stat, Is.EqualTo(hudToolViewModelExpected.Stats.First().Stat));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutTextBoxTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutTextBoxToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutTextBoxTool
            {
                Text = "Test"
            };

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.Text, Is.EqualTo(hudLayoutToolExpected.Text));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
        }

        /// <summary>
        /// Method tests if <see cref="HudTextBoxViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudTextBoxViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutTextBoxTool();

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudTextBoxViewModel;
            hudToolViewModelExpected.Width = 100;
            hudToolViewModelExpected.Height = 75;
            hudToolViewModelExpected.Position = new System.Windows.Point(5, 5);
            hudToolViewModelExpected.Opacity = 60;
            hudToolViewModelExpected.Text = "Test";

            Assert.IsNotNull(hudToolViewModelExpected);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Opacity, Is.EqualTo(hudToolViewModelExpected.Opacity));
            Assert.That(hudToolViewModelActual.Text, Is.EqualTo(hudToolViewModelExpected.Text));
        }

        /// <summary>
        /// Method tests if <see cref="HudLayoutBumperStickersTool"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutBumperStickersToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutBumperStickersTool();

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.ToolType, Is.EqualTo(hudLayoutToolExpected.ToolType));
        }

        /// <summary>
        /// Method tests if <see cref="HudBumperStickersViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudBumperStickersViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutBumperStickersTool();

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudBumperStickersViewModel;
            hudToolViewModelExpected.Width = 100;
            hudToolViewModelExpected.Height = 75;
            hudToolViewModelExpected.Position = new System.Windows.Point(5, 5);
            hudToolViewModelExpected.Opacity = 60;

            Assert.IsNotNull(hudToolViewModelExpected);

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.Opacity, Is.EqualTo(hudToolViewModelExpected.Opacity));
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
        /// Method tests if <see cref="HudLayoutGaugeIndicator"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudLayoutHeatMapToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutHeatMapTool
            {
                BaseStat = new StatInfo { Stat = Stat.S3Bet }
            };

            var hudLayoutToolActual = SerializerHelper.GetSerializedDeserializedObject(hudLayoutToolExpected);

            Assert.That(hudLayoutToolActual.Id, Is.EqualTo(hudLayoutToolExpected.Id));
            Assert.That(hudLayoutToolActual.BaseStat.Stat, Is.EqualTo(hudLayoutToolExpected.BaseStat.Stat));
        }

        /// <summary>
        /// Method tests if <see cref="HudHeatMapViewModel"/> can be serialized with <seealso cref="Serializer"/>
        /// </summary>
        [Test]
        public void HudHeatMapViewModelToolCanBeSerializedDeserializedWithProtobuf()
        {
            var hudLayoutToolExpected = new HudLayoutHeatMapTool
            {
                BaseStat = new StatInfo { Stat = Stat.S3Bet }
            };

            var hudElement = new HudElementViewModel
            {
                Seat = 1,
            };

            var hudToolViewModelExpected = hudLayoutToolExpected.CreateViewModel(hudElement) as HudHeatMapViewModel;

            var heatMap = new HeatMapDto();
            heatMap.OccuredByCardRange.Add("AA", 10);

            hudToolViewModelExpected.HeatMap = heatMap;

            var hudToolViewModelActual = SerializerHelper.GetSerializedDeserializedObject(hudToolViewModelExpected);

            Assert.That(hudToolViewModelActual.Id, Is.EqualTo(hudToolViewModelExpected.Id));
            Assert.That(hudToolViewModelActual.ToolType, Is.EqualTo(hudToolViewModelExpected.ToolType));
            Assert.That(hudToolViewModelActual.BaseStat.Stat, Is.EqualTo(hudToolViewModelExpected.BaseStat.Stat));
            Assert.That(hudToolViewModelActual.Width, Is.EqualTo(hudToolViewModelExpected.Width));
            Assert.That(hudToolViewModelActual.Height, Is.EqualTo(hudToolViewModelExpected.Height));
            Assert.That(hudToolViewModelActual.Position, Is.EqualTo(hudToolViewModelExpected.Position));
            Assert.That(hudToolViewModelActual.HeatMap.OccuredByCardRange.Count, Is.EqualTo(hudToolViewModelExpected.HeatMap.OccuredByCardRange.Count));
            Assert.That(hudToolViewModelActual.HeatMap.OccuredByCardRange.Keys.First(), Is.EqualTo(hudToolViewModelExpected.HeatMap.OccuredByCardRange.Keys.First()));
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
                Name = "TestLayout",
                Filter = new HudLayoutFilter
                {
                    DataFreshness = 30,
                    TableTypes = new[] { (int)EnumTableType.HU, (int)EnumTableType.Six }
                },
                TrackMeterPositions = new List<HudPositionsInfo>
                {
                    new HudPositionsInfo
                    {
                         GameType = EnumGameType.MTTHoldem,
                         PokerSite = EnumPokerSites.Bodog,
                         HudPositions = new List<HudPositionInfo>
                         {
                            new HudPositionInfo
                            {
                                 Position = new System.Windows.Point(1,15)
                            }
                         }
                    }
                }
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
            var playerIconTool = new HudLayoutPlayerIconTool();
            var graphTool = new HudLayoutGraphTool();
            var gaugeIndicatorTool = new HudLayoutGaugeIndicator();
            var bumperStickers = new HudLayoutBumperStickersTool();
            var heatMap = new HudLayoutHeatMapTool();

            hudLayoutInfo.LayoutTools = new List<HudLayoutTool> { plainBoxTool, textboxTool, fourStatBoxTool, tiltMeterTool, playerIconTool, graphTool, gaugeIndicatorTool, bumperStickers, heatMap };

            return hudLayoutInfo;
        }
    }
}