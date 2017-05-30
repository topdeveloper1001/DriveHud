//-----------------------------------------------------------------------
// <copyright file="StatInfoTests.cs" company="Ace Poker Solutions">
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
using Model.Data;
using Model.Enums;
using Model.Stats;
using NUnit.Framework;
using ProtoBuf.Meta;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class StatInfoTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            RuntimeTypeModel.Default.Add(typeof(Color), false).SetSurrogate(typeof(ColorDto));
            RuntimeTypeModel.Default.Add(typeof(SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Point), false).SetSurrogate(typeof(PointDto));
        }

        [Test]
        public void TestStatInfoSerialization()
        {
            var expectedStatInfo = new StatInfo
            {
                Stat = Stat.S3Bet,
                Caption = "Caption1",
                IsCaptionHidden = true,
                Format = "Format1",
                PropertyName = "PropertyName1",
                CurrentColor = Colors.Black,
                CurrentValue = 15m,
                SettingsAppearance_IsChecked = true,
                SettingsPlayerType_IsChecked = true,
                SettingsAppearanceFontSource = "Font",
                SettingsAppearanceFontSize = 15,
                SettingsAppearanceFontBold_IsChecked = true,
                SettingsAppearanceFontItalic_IsChecked = true,
                SettingsAppearanceFontUnderline_IsChecked = true,
                SettingsAppearanceValueRangeCollection = new ObservableCollection<StatInfoOptionValueRange>()
                {
                    new StatInfoOptionValueRange()
                },
                MinSample = 15,
                Label = "Label1",
                IsNotVisible = true,
                IsListed = true,
                StatDto = new StatDto(1, 10),
                StatInfoMeter = new StatInfoMeterModel(),
                GraphToolIconSource = "GraphTool"
            };

            var actualStatInfo = SerializerHelper.GetSerializedDeserializedObject(expectedStatInfo);

            Assert.Multiple(() =>
            {
                Assert.That(actualStatInfo.Id, Is.EqualTo(expectedStatInfo.Id), nameof(StatInfo.Id));
                Assert.That(actualStatInfo.Stat, Is.EqualTo(expectedStatInfo.Stat), nameof(StatInfo.Stat));
                Assert.That(actualStatInfo.Caption, Is.EqualTo(expectedStatInfo.Caption), nameof(StatInfo.Caption));
                Assert.That(actualStatInfo.IsCaptionHidden, Is.EqualTo(expectedStatInfo.IsCaptionHidden), nameof(StatInfo.IsCaptionHidden));
                Assert.That(actualStatInfo.Format, Is.EqualTo(expectedStatInfo.Format), nameof(StatInfo.Format));
                Assert.That(actualStatInfo.PropertyName, Is.EqualTo(expectedStatInfo.PropertyName), nameof(StatInfo.PropertyName));
                Assert.That(actualStatInfo.CurrentColor, Is.EqualTo(expectedStatInfo.CurrentColor), nameof(StatInfo.CurrentColor));
                Assert.That(actualStatInfo.CurrentValue, Is.EqualTo(expectedStatInfo.CurrentValue), nameof(StatInfo.CurrentValue));
                Assert.That(actualStatInfo.SettingsAppearance_IsChecked, Is.EqualTo(expectedStatInfo.SettingsAppearance_IsChecked), nameof(StatInfo.SettingsAppearance_IsChecked));
                Assert.That(actualStatInfo.SettingsPlayerType_IsChecked, Is.EqualTo(expectedStatInfo.SettingsPlayerType_IsChecked), nameof(StatInfo.SettingsPlayerType_IsChecked));
                Assert.That(actualStatInfo.SettingsAppearanceFontSource, Is.EqualTo(expectedStatInfo.SettingsAppearanceFontSource), nameof(StatInfo.SettingsAppearanceFontSource));
                Assert.That(actualStatInfo.SettingsAppearanceFontSize, Is.EqualTo(expectedStatInfo.SettingsAppearanceFontSize), nameof(StatInfo.SettingsAppearanceFontSize));
                Assert.That(actualStatInfo.SettingsAppearanceFontBold_IsChecked, Is.EqualTo(expectedStatInfo.SettingsAppearanceFontBold_IsChecked), nameof(StatInfo.SettingsAppearanceFontBold_IsChecked));
                Assert.That(actualStatInfo.SettingsAppearanceFontItalic_IsChecked, Is.EqualTo(expectedStatInfo.SettingsAppearanceFontItalic_IsChecked), nameof(StatInfo.SettingsAppearanceFontItalic_IsChecked));
                Assert.That(actualStatInfo.SettingsAppearanceFontUnderline_IsChecked, Is.EqualTo(expectedStatInfo.SettingsAppearanceFontUnderline_IsChecked), nameof(StatInfo.SettingsAppearanceFontUnderline_IsChecked));
                Assert.That(actualStatInfo.SettingsAppearanceValueRangeCollection.Count, Is.EqualTo(expectedStatInfo.SettingsAppearanceValueRangeCollection.Count), nameof(StatInfo.SettingsAppearanceValueRangeCollection));
                Assert.That(actualStatInfo.MinSample, Is.EqualTo(expectedStatInfo.MinSample), nameof(StatInfo.MinSample));
                Assert.That(actualStatInfo.Label, Is.EqualTo(expectedStatInfo.Label), nameof(StatInfo.Label));
                Assert.That(actualStatInfo.IsNotVisible, Is.EqualTo(expectedStatInfo.IsNotVisible), nameof(StatInfo.IsNotVisible));
                Assert.That(actualStatInfo.IsListed, Is.EqualTo(expectedStatInfo.IsListed), nameof(StatInfo.IsListed));
                Assert.That(actualStatInfo.StatDto.Occurred, Is.EqualTo(expectedStatInfo.StatDto.Occurred), nameof(StatDto.Occurred));
                Assert.That(actualStatInfo.StatDto.CouldOccurred, Is.EqualTo(expectedStatInfo.StatDto.CouldOccurred), nameof(StatDto.CouldOccurred));
                CollectionAssert.AreEqual(expectedStatInfo.StatInfoMeter.BackgroundBrush, actualStatInfo.StatInfoMeter.BackgroundBrush, new SolidBrushComparer(), nameof(StatInfoMeterModel.BackgroundBrush));
                CollectionAssert.AreEqual(expectedStatInfo.StatInfoMeter.BorderBrush, actualStatInfo.StatInfoMeter.BorderBrush, new SolidBrushComparer(), nameof(StatInfoMeterModel.BorderBrush));
                Assert.That(actualStatInfo.GraphToolIconSource, Is.EqualTo(expectedStatInfo.GraphToolIconSource), nameof(StatInfo.GraphToolIconSource));
            });
        }

        private class SolidBrushComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var brushX = x as SolidColorBrush;
                var brushY = y as SolidColorBrush;

                if (brushX == null || brushY == null)
                {
                    return -1;
                }

                if (brushX.Color == brushY.Color)
                {
                    return 0;
                }

                return -1;
            }
        }
    }
}