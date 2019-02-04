//-----------------------------------------------------------------------
// <copyright file="FiltersModelSerializationTests.cs" company="Ace Poker Solutions">
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
using DriveHUD.Application.ViewModels.Hud;
using HandHistories.Objects.Actions;
using Model.Filters;
using Model.Hud;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class FiltersModelSerializationTests
    {
        private static string testDataFolder;

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            testDataFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "UnitTests", "TestData", "FilterModelSerializedData");
        }

        [Test]
        public void FilterHoleCardsModelSerializeTest()
        {
            var filterHoleCardsModelExpected = new FilterHoleCardsModel();
            filterHoleCardsModelExpected.Initialize();

            var aaHoleCards = filterHoleCardsModelExpected.HoleCardsCollection.FirstOrDefault(x => x.Name == "AA");

            Assert.IsNotNull(aaHoleCards);

            aaHoleCards.IsChecked = false;

            var filterHoleCardsModelActual = SerializerHelper.GetXmlSerializedDeserializedObject(filterHoleCardsModelExpected);

            Assert.That(filterHoleCardsModelActual.Id, Is.EqualTo(filterHoleCardsModelExpected.Id));
            Assert.That(filterHoleCardsModelActual.Type, Is.EqualTo(filterHoleCardsModelExpected.Type));
            Assert.That(filterHoleCardsModelActual.HoleCardsCollection.Count, Is.EqualTo(filterHoleCardsModelExpected.HoleCardsCollection.Count));
            Assert.That(filterHoleCardsModelActual.HoleCardsCollection.Count(x => !x.IsChecked), Is.EqualTo(filterHoleCardsModelExpected.HoleCardsCollection.Count(x => !x.IsChecked)));
        }

        [Test]
        [TestCase("FilterHoleCardsModel.xml")]
        public void FilterHoleCardsModelDeserializeTest(string fileName)
        {
            var file = Path.Combine(testDataFolder, fileName);

            var serializer = new XmlSerializer(typeof(FilterHoleCardsModel));

            Assert.DoesNotThrow(() =>
            {
                using (var sr = new StreamReader(file))
                {
                    var filterHoleCardsModel = (FilterHoleCardsModel)serializer.Deserialize(sr);
                }
            });
        }

        [Test]
        public void HudBumperStickerTypeFilterModelCollectionSerializeTest()
        {
            var filterHoleCardsModelExpected = new FilterHoleCardsModel();
            filterHoleCardsModelExpected.Initialize();

            var filterHandValueModel = new FilterHandValueModel();
            filterHandValueModel.Initialize();

            var filterHandActionModelExpected = new FilterHandActionModel();
            filterHandActionModelExpected.Initialize();

            var hudBumperStickerTypeExpected = new HudBumperStickerType();
            hudBumperStickerTypeExpected.FilterModelCollection = new IFilterModelCollection { filterHoleCardsModelExpected, filterHandValueModel, filterHandActionModelExpected };

            var hudBumperStickerTypeActual = SerializerHelper.GetXmlSerializedDeserializedObject(hudBumperStickerTypeExpected);

            Assert.That(hudBumperStickerTypeActual.FilterModelCollection.Count, Is.EqualTo(hudBumperStickerTypeExpected.FilterModelCollection.Count));
        }

        [Test]
        public void FilterHandValueModelSerializeTest()
        {
            var filterHandValueModelExpected = new FilterHandValueModel();
            filterHandValueModelExpected.Initialize();

            var fastFilterItem = filterHandValueModelExpected.FastFilterCollection.FirstOrDefault();
            fastFilterItem.CurrentTriState = Model.Enums.EnumTriState.Off;

            var flopHandValueItem = filterHandValueModelExpected.FlopHandValuesCollection.FirstOrDefault();
            flopHandValueItem.IsChecked = true;

            var turnHandValueItem = filterHandValueModelExpected.TurnHandValuesCollection.FirstOrDefault();
            turnHandValueItem.IsChecked = true;

            var riverHandValueItem = filterHandValueModelExpected.RiverHandValuesCollection.FirstOrDefault();
            riverHandValueItem.IsChecked = true;

            var filterHandValueModelActual = SerializerHelper.GetXmlSerializedDeserializedObject(filterHandValueModelExpected);

            Assert.That(filterHandValueModelActual.Id, Is.EqualTo(filterHandValueModelExpected.Id));
            Assert.That(filterHandValueModelActual.Type, Is.EqualTo(filterHandValueModelExpected.Type));

            Assert.That(filterHandValueModelActual.FastFilterCollection.Count, Is.EqualTo(filterHandValueModelExpected.FastFilterCollection.Count));
            Assert.That(filterHandValueModelActual.FastFilterCollection.Count(x => x.CurrentTriState != Model.Enums.EnumTriState.Any), Is.EqualTo(filterHandValueModelExpected.FastFilterCollection.Count(x => x.CurrentTriState != Model.Enums.EnumTriState.Any)));

            Assert.That(filterHandValueModelActual.FlopHandValuesCollection.Count, Is.EqualTo(filterHandValueModelExpected.FlopHandValuesCollection.Count));
            Assert.That(filterHandValueModelActual.FlopHandValuesCollection.Count(x => x.IsChecked), Is.EqualTo(filterHandValueModelExpected.FlopHandValuesCollection.Count(x => x.IsChecked)));

            Assert.That(filterHandValueModelActual.TurnHandValuesCollection.Count, Is.EqualTo(filterHandValueModelExpected.TurnHandValuesCollection.Count));
            Assert.That(filterHandValueModelActual.TurnHandValuesCollection.Count(x => x.IsChecked), Is.EqualTo(filterHandValueModelExpected.TurnHandValuesCollection.Count(x => x.IsChecked)));

            Assert.That(filterHandValueModelActual.RiverHandValuesCollection.Count, Is.EqualTo(filterHandValueModelExpected.RiverHandValuesCollection.Count));
            Assert.That(filterHandValueModelActual.RiverHandValuesCollection.Count(x => x.IsChecked), Is.EqualTo(filterHandValueModelExpected.RiverHandValuesCollection.Count(x => x.IsChecked)));
        }

        [Test]
        public void FilterHandActionModelSerializeTest()
        {
            var filterHandActionModelExpected = new FilterHandActionModel();
            filterHandActionModelExpected.Initialize();

            filterHandActionModelExpected.PreflopItems.FirstOrDefault().IsChecked = true;
            filterHandActionModelExpected.PreflopButtons.FirstOrDefault().IsChecked = true;
            filterHandActionModelExpected.FlopItems.FirstOrDefault().IsChecked = true;
            filterHandActionModelExpected.FlopButtons.FirstOrDefault().IsChecked = true;

            var turnItemExpected = filterHandActionModelExpected.TurnItems.FirstOrDefault(x => x.Name == "Then Call Then Raise Then Fold" && x.BeginningActionType == HandActionType.BET);
            turnItemExpected.IsChecked = true;

            var turnButtonExpected = filterHandActionModelExpected.TurnButtons.FirstOrDefault(x => x.HandActionType == HandActionType.BET);
            turnButtonExpected.IsChecked = true;

            filterHandActionModelExpected.RiverItems.FirstOrDefault().IsChecked = true;
            filterHandActionModelExpected.RiverButtons.FirstOrDefault().IsChecked = true;

            var filterHandActionModelActual = SerializerHelper.GetXmlSerializedDeserializedObject(filterHandActionModelExpected);

            Assert.That(filterHandActionModelActual.Id, Is.EqualTo(filterHandActionModelExpected.Id));
            Assert.That(filterHandActionModelActual.Type, Is.EqualTo(filterHandActionModelExpected.Type));

            Assert.That(filterHandActionModelActual.PreflopItems.Count, Is.EqualTo(filterHandActionModelExpected.PreflopItems.Count));
            Assert.That(filterHandActionModelActual.PreflopItems.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.PreflopItems.Count(x => x.IsChecked)));

            Assert.That(filterHandActionModelActual.PreflopButtons.Count, Is.EqualTo(filterHandActionModelExpected.PreflopButtons.Count));
            Assert.That(filterHandActionModelActual.PreflopButtons.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.PreflopButtons.Count(x => x.IsChecked)));

            Assert.That(filterHandActionModelActual.FlopItems.Count, Is.EqualTo(filterHandActionModelExpected.FlopItems.Count));
            Assert.That(filterHandActionModelActual.FlopItems.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.FlopItems.Count(x => x.IsChecked)));

            Assert.That(filterHandActionModelActual.FlopButtons.Count, Is.EqualTo(filterHandActionModelExpected.FlopButtons.Count));
            Assert.That(filterHandActionModelActual.FlopButtons.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.FlopButtons.Count(x => x.IsChecked)));

            Assert.That(filterHandActionModelActual.TurnItems.Count, Is.EqualTo(filterHandActionModelExpected.TurnItems.Count));
            Assert.That(filterHandActionModelActual.TurnItems.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.TurnItems.Count(x => x.IsChecked)));

            var turnItemActual = filterHandActionModelActual.TurnItems.FirstOrDefault(x => x.Name == "Then Call Then Raise Then Fold" && x.BeginningActionType == HandActionType.BET);
            var turnButtonActual = filterHandActionModelActual.TurnButtons.FirstOrDefault(x => x.HandActionType == HandActionType.BET);

            Assert.That(turnItemExpected.Id, Is.EqualTo(turnItemActual.Id));
            Assert.That(turnItemExpected.IsChecked, Is.EqualTo(turnItemActual.IsChecked));
            Assert.That(turnItemExpected.BeginningActionType, Is.EqualTo(turnItemActual.BeginningActionType));
            Assert.That(turnItemExpected.ActionString, Is.EqualTo(turnItemActual.ActionString));

            Assert.That(turnButtonExpected.Id, Is.EqualTo(turnButtonActual.Id));
            Assert.That(turnButtonExpected.IsChecked, Is.EqualTo(turnButtonActual.IsChecked));
            Assert.That(turnButtonExpected.HandActionType, Is.EqualTo(turnButtonActual.HandActionType));

            Assert.That(filterHandActionModelActual.TurnButtons.Count, Is.EqualTo(filterHandActionModelExpected.TurnButtons.Count));
            Assert.That(filterHandActionModelActual.TurnButtons.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.TurnButtons.Count(x => x.IsChecked)));

            Assert.That(filterHandActionModelActual.RiverItems.Count, Is.EqualTo(filterHandActionModelExpected.RiverItems.Count));
            Assert.That(filterHandActionModelActual.RiverItems.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.RiverItems.Count(x => x.IsChecked)));

            Assert.That(filterHandActionModelActual.RiverButtons.Count, Is.EqualTo(filterHandActionModelExpected.RiverButtons.Count));
            Assert.That(filterHandActionModelActual.RiverButtons.Count(x => x.IsChecked), Is.EqualTo(filterHandActionModelExpected.RiverButtons.Count(x => x.IsChecked)));
        }
    }
}