//-----------------------------------------------------------------------
// <copyright file="HudLayoutsSerialization.cs" company="Ace Poker Solutions">
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
using DriveHUD.ViewModels;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using System.IO;
using System.Windows.Media;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]

    class HudLayoutsSerialization
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.Color), false).SetSurrogate(typeof(ColorDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
        }

        [Test]
        public void TestColorSerialization()
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

        [Test]
        public void TestStatInfoMeterModelSerialization()
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
    }
}