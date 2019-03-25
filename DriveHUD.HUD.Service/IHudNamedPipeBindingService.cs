//-----------------------------------------------------------------------
// <copyright file="IHudNamedPipeBindingService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows;

namespace DriveHUD.HUD.Service
{
    [ServiceContract(Name = "IHudNamedPipeBindingService", SessionMode = SessionMode.Required, CallbackContract = typeof(IHudNamedPipeBindingCallbackService))]
    public interface IHudNamedPipeBindingService
    {
        [OperationContract(Name = "UpdateHUD", IsOneWay = true)]
        void UpdateHUD(byte[] data);

        [OperationContract(Name = "CloseTable", IsOneWay = true)]
        void CloseTable(int windowHandle);

        #region Callback connection manager

        [OperationContract(Name = "ConnectCallbackChannel", IsOneWay = true)]
        void ConnectCallbackChannel(string name);
   
        #endregion
    }

    public interface IHudNamedPipeBindingCallbackService
    {
        [OperationContract(Name = "SaveHudLayout", IsOneWay = true)]
        void SaveHudLayout(HudLayoutContract hudLayout);

        [OperationContract(Name = "ReplayHand", IsOneWay = true)]
        void ReplayHand(long gameNumber, short pokerSiteId);

        [OperationContract(Name = "LoadLayout", IsOneWay = true)]
        void LoadLayout(string layoutName, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType);

        [OperationContract(Name = "TagHands", IsOneWay = true)]
        void TagHands(IEnumerable<long> gameNumber, short pokerSiteId, int tag);

        [OperationContract(Name = "TreatTableAs", IsOneWay = true)]
        void TreatTableAs(IntPtr handle, EnumTableType tableType);
    }

    [DataContract]
    public class HudLayoutContract
    {
        [DataMember(Name = "LayoutName")]
        public string LayoutName { get; set; }

        [DataMember(Name = "PokerSite")]
        public EnumPokerSites PokerSite { get; set; }

        [DataMember(Name = "GameType")]
        public EnumGameType GameType { get; set; }

        [DataMember(Name = "TableType")]
        public EnumTableType TableType { get; set; }

        [DataMember(Name = "HudPositions")]
        public List<HudPositionContract> HudPositions { get; set; }

        [DataMember(Name = "TrackMeterPosition")]
        public Point TrackMeterPosition { get; set; }
    }

    [DataContract]
    public class HudPositionContract
    {
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "SeatNumber")]
        public int SeatNumber { get; set; }

        [DataMember(Name = "Position")]
        public Point Position { get; set; }
    }
}