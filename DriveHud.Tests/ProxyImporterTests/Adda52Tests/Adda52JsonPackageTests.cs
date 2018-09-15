//-----------------------------------------------------------------------
// <copyright file="Adda52JsonPackageTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Adda52.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace DriveHud.Tests.ProxyImporterTests.Adda52Tests
{
    [TestFixture]
    class Adda52JsonPackageTests
    {
        [TestCaseSource("TestAdd52Packages")]
        public void TryParseTest(string json, Adda52PackageType packageType)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            var result = Adda52JsonPackage.TryParse(jsonBytes, out Adda52JsonPackage jsonPackage);

            Assert.IsTrue(result);
            Assert.That(jsonPackage.PackageType, Is.EqualTo(packageType));
        }

        private static IEnumerable<TestCaseData> TestAdd52Packages()
        {
            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.seatinfo\",\"r\":50441,\"p\":{\"isOnSitout\":[true,true,true,true,true,false,true,false,false],\"roomName\":\"MTT9#3184578\",\"useConverter\":false,\"seatinfo\":{\"seats\":[{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Favatar21\",\"away\":false,\"bet\":0,\"chips\":3830,\"chipsLeft\":3830,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1561705,\"playerName\":\"Archnarani\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":1,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar42\",\"away\":true,\"bet\":0,\"chips\":203,\"chipsLeft\":203,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1451879,\"playerName\":\"confuzdtruth\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":2,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar33\",\"away\":false,\"bet\":0,\"chips\":9587,\"chipsLeft\":9587,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1516896,\"playerName\":\"Vihu2016\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":3,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar49\",\"away\":true,\"bet\":0,\"chips\":168,\"chipsLeft\":168,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1760705,\"playerName\":\"Jineeshks3\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":4,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar62\",\"away\":true,\"bet\":0,\"chips\":598,\"chipsLeft\":598,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1756660,\"playerName\":\"Thepk\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":5,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar32\",\"away\":false,\"bet\":0,\"chips\":34203,\"chipsLeft\":34203,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":false,\"playerId\":1759798,\"playerName\":\"Komalpreet0003\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":6,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar22\",\"away\":false,\"bet\":0,\"chips\":10982,\"chipsLeft\":10982,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1456254,\"playerName\":\"123cool\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":7,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mguy5\",\"away\":false,\"bet\":0,\"chips\":34688,\"chipsLeft\":34688,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":false,\"playerId\":1490662,\"playerName\":\"lalhmaacesc\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":8,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false},{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar23\",\"away\":false,\"bet\":0,\"chips\":1938,\"chipsLeft\":1938,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":false,\"playerId\":729464,\"playerName\":\"hellmute\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":9,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false}],\"sprintAvailableSeat\":null,\"unreservedSeat\":null,\"zoomSitoutSeat\":{\"actionTakenInThisRound\":false,\"addOn\":false,\"addOnAllowed\":false,\"allIn\":false,\"avatar\":\"Mavatar42\",\"away\":true,\"bet\":0,\"chips\":203,\"chipsLeft\":203,\"chipsType\":\"\",\"folded\":false,\"lostAllin\":false,\"lostAllinPlayerId\":-1,\"mucked\":false,\"occupied\":true,\"offeredAddon\":false,\"offeredRebuy\":false,\"onSitout\":true,\"playerId\":1451879,\"playerName\":\"confuzdtruth\",\"playing\":1,\"rebuy\":false,\"rebuyAllowed\":false,\"reserved\":false,\"seatId\":2,\"seatLock\":{\"fair\":true,\"queueLength\":0,\"readHoldCount\":0,\"readLockCount\":0,\"writeHoldCount\":0,\"writeLocked\":false,\"writeLockedByCurrentThread\":false},\"sitoutHandle\":null,\"sprintReserved\":false}}}},\"a\":13}",
                Adda52PackageType.SeatInfo)
            { TestName = "TryParseSeatInfoTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.roomdata\",\"r\":50441,\"p\":{\"chipType\":\"PP_71362\",\"turnTime\":15,\"isCoolOff\":false,\"isAnonynounsTable\":false,\"gameType\":\"MTT\",\"isTimeBank\":true,\"useConverter\":false,\"bettingRule\":\"NL\",\"coolOffTime\":0,\"minPlayers\":2,\"isSprintSitOutRoom\":false,\"isDynamic\":false,\"isRebuy\":true,\"ringVariant\":\"HOLDEM\",\"amount\":\"1#2000#,\",\"timeBank\":15,\"isZoom\":false,\"bigBlind\":20,\"roomName\":\"MTT9#3184578\",\"buyInHigh\":2000,\"players\":9,\"rakeNonHeadsUp\":0,\"timeBankMaxHands\":20,\"buyInLow\":2000,\"smallBlind\":10}},\"a\":13}",
                Adda52PackageType.RoomData)
            { TestName = "TryParseRoomDataTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.antetaken\",\"r\":50441,\"p\":{\"amount\":360,\"roomName\":\"MTT9#3184578\",\"useConverter\":false}},\"a\":13}",
                Adda52PackageType.Ante)
            { TestName = "TryParseAnteTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.Blinds\",\"r\":50441,\"p\":{\"roomName\":\"MTT9#3184578\",\"useConverter\":false,\"sb\":200,\"bb\":400}},\"a\":13}",
                Adda52PackageType.Blinds)
            { TestName = "TryParseBlindsTest" };


            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.Dealer\",\"r\":50441,\"p\":{\"dealer\":8,\"timestamp\":\"2018 - 09 - 14 17:59:44:532\",\"roomName\":\"MTT9#3184578\",\"useConverter\":false}},\"a\":13}",
                Adda52PackageType.Dealer)
            { TestName = "TryParseDealerTest" };


            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.useraction\",\"r\":50441,\"p\":{\"timestamp\":\"2018 - 09 - 14 17:59:44:532\",\"amt\":200,\"totAmt\":200,\"flagToCheckBet\":false,\"roomName\":\"MTT9#3184578\",\"useConverter\":false,\"action\":11,\"playerid\":729464,\"chipsLeft\":1698,\"maxRaiseAmount\":-1}},\"a\":13}",
                Adda52PackageType.UserAction)
            { TestName = "TryParseUserActionTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.started\",\"r\":50441,\"p\":{\"roomName\":\"MTT9#3184578\",\"useConverter\":false,\"roundId\":55}},\"a\":13}",
                Adda52PackageType.GameStart)
            { TestName = "TryParseGameStartTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.roundend\",\"r\":50441,\"p\":{\"roomName\":\"MTT9#3184578\",\"useConverter\":false,\"delay\":2}},\"a\":13}",
                Adda52PackageType.RoundEnd)
            { TestName = "TryParseRoundEndTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.communitycard\",\"r\":50441,\"p\":{\"CommunityCard\":{\"communityCards\":[{\"face\":{\"text\":\"5\",\"value\":5},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}},{\"face\":{\"text\":\"3\",\"value\":3},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}}],\"multiRun\":false,\"multiRunCommCards\":[]},\"roomName\":\"MTT9#3184578\",\"useConverter\":false}},\"a\":13}",
                Adda52PackageType.CommunityCard)
            { TestName = "TryParseCommunityCardTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.winner\",\"r\":50441,\"p\":{\"timestamp\":\"2018 - 09 - 14 18:00:23:875\",\"duration\":8,\"WinnerInfo\":{\"lowCardWinnerInfo\":false,\"notifyMultiRun\":false,\"playerRankingList\":[{\"holeCards\":{\"card1\":{\"face\":{\"text\":\"k\",\"value\":13},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},\"card2\":{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"h\",\"value\":2}},\"card3\":null,\"card4\":null,\"holeCards\":[{\"face\":{\"text\":\"k\",\"value\":13},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"h\",\"value\":2}}]},\"kickerCards\":[{\"face\":{\"text\":\"k\",\"value\":13},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"j\",\"value\":11},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"5\",\"value\":5},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}}],\"kickerCardsIndex\":0,\"muckTeaseAction\":3,\"playerId\":1451879,\"rank\":{\"name\":\"One Pair\",\"value\":2},\"rankCards\":[{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"h\",\"value\":2}},{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}}],\"rankCardsIndex\":0},{\"holeCards\":{\"card1\":{\"face\":{\"text\":\"10\",\"value\":10},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}},\"card2\":{\"face\":{\"text\":\"3\",\"value\":3},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},\"card3\":null,\"card4\":null,\"holeCards\":[{\"face\":{\"text\":\"10\",\"value\":10},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}},{\"face\":{\"text\":\"3\",\"value\":3},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}}]},\"kickerCards\":[{\"face\":{\"text\":\"j\",\"value\":11},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"10\",\"value\":10},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}},{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}}],\"kickerCardsIndex\":0,\"muckTeaseAction\":3,\"playerId\":1759798,\"rank\":{\"name\":\"One Pair\",\"value\":2},\"rankCards\":[{\"face\":{\"text\":\"3\",\"value\":3},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"3\",\"value\":3},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}}],\"rankCardsIndex\":0},{\"holeCards\":{\"card1\":{\"face\":{\"text\":\"5\",\"value\":5},\"multiRunCard\":false,\"suit\":{\"text\":\"h\",\"value\":2}},\"card2\":{\"face\":{\"text\":\"1\",\"value\":14},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},\"card3\":null,\"card4\":null,\"holeCards\":[{\"face\":{\"text\":\"5\",\"value\":5},\"multiRunCard\":false,\"suit\":{\"text\":\"h\",\"value\":2}},{\"face\":{\"text\":\"1\",\"value\":14},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}}]},\"kickerCards\":[{\"face\":{\"text\":\"1\",\"value\":14},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"j\",\"value\":11},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}},{\"face\":{\"text\":\"6\",\"value\":6},\"multiRunCard\":false,\"suit\":{\"text\":\"c\",\"value\":0}}],\"kickerCardsIndex\":0,\"muckTeaseAction\":3,\"playerId\":729464,\"rank\":{\"name\":\"One Pair\",\"value\":2},\"rankCards\":[{\"face\":{\"text\":\"5\",\"value\":5},\"multiRunCard\":false,\"suit\":{\"text\":\"h\",\"value\":2}},{\"face\":{\"text\":\"5\",\"value\":5},\"multiRunCard\":false,\"suit\":{\"text\":\"d\",\"value\":1}}],\"rankCardsIndex\":0}],\"toShowLowPotWinnerNotExits\":false,\"winnerList\":[{\"amount\":852,\"playerId\":1451879,\"playerPotBuyinAmount\":284,\"potId\":0},{\"amount\":3147,\"playerId\":729464,\"playerPotBuyinAmount\":1573,\"potId\":1}]},\"roomName\":\"MTT9#3184578\",\"useConverter\":false}},\"a\":13}",
                Adda52PackageType.Winner)
            { TestName = "TryParseWinnerTest" };

            yield return new TestCaseData(
                "{\"c\":1,\"p\":{\"c\":\"game.returnPotMoney\",\"r\":50441,\"p\":{\"amount\":33763,\"roomName\":\"MTT9#3184578\",\"useConverter\":false,\"seatId\":6,\"toBeCredited\":true,\"updateStatus\":false,\"chipsLeft\":33763}},\"a\":13}",
                Adda52PackageType.UncalledBet)
            { TestName = "TryParseUncalledBetTest" };
        }
    }
}