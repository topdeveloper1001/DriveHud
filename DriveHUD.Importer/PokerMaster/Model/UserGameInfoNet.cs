//-----------------------------------------------------------------------
// <copyright file="UserGameInfoNet.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;

namespace DriveHUD.Importers.PokerMaster.Model
{
    [ProtoContract]
    internal class UserGameInfoNet
    {
        [ProtoMember(1)]
        public UserBaseInfoNet UserInfo { get; set; }

        [ProtoMember(2)]
        public UserGameStates GameState { get; set; }

        [ProtoMember(3)]
        public UserGameRoles GameRole { get; set; }

        [ProtoMember(4)]
        public long RoomId { get; set; }

        [ProtoMember(5)]
        public long RemainStacks { get; set; }

        [ProtoMember(6)]
        public bool HasBuyin { get; set; }

        [ProtoMember(7)]
        public long BetStacks { get; set; }

        [ProtoMember(8)]
        public int[] CurrentHands { get; set; }

        public bool HasKnownCards
        {
            get
            {
                return CurrentHands != null;
            }
        }

        [ProtoMember(9)]
        public bool GameDealer { get; set; }

        [ProtoMember(10)]
        public int DelayTotalTimes { get; set; }

        [ProtoMember(11)]
        public long ActTime { get; set; }

        [ProtoMember(12)]
        public long DelayLong { get; set; }

        [ProtoMember(13)]
        public int DelayCost { get; set; }

        [ProtoMember(14)]
        public int DelaySingleTime { get; set; }

        [ProtoMember(15)]
        public bool ShowCardOne { get; set; }

        [ProtoMember(16)]
        public bool ShowCardTwo { get; set; }

        [ProtoMember(17)]
        public long BettingID { get; set; }

        [ProtoMember(18)]
        public int BuyinStacks { get; set; }

        [ProtoMember(19)]
        public bool WaitingBuyinConfirmation { get; set; }

        [ProtoMember(20)]
        public long BuyinTime { get; set; }

        [ProtoMember(21)]
        public int SNGRank { get; set; }

        [ProtoMember(22)]
        public bool Delegate { get; set; }

        [ProtoMember(23)]
        public bool WaitBB { get; set; }

        [ProtoMember(24)]
        public bool JustSit { get; set; }

        [ProtoMember(25)]
        public bool ShowCardThree { get; set; }

        [ProtoMember(26)]
        public bool ShowCardFour { get; set; }

        [ProtoMember(27)]
        public bool KickedInGame { get; set; }

        [ProtoMember(28)]
        public long BuyinClubID { get; set; }

        [ProtoMember(29)]
        public int[] PayTableCards { get; set; }

        [ProtoMember(30)]
        public UserGameStates PreAction { get; set; }

        [ProtoMember(31)]
        public int UserHandID { get; set; }

        public GameRoomGameState RoomGameState { get; set; }

        public bool IsActive
        {
            get
            {
                return GameState != UserGameStates.USER_GAME_STATE_WAIT && 
                    GameState != UserGameStates.USER_GAME_STATE_STANDBY;
            }
        }
    }
}