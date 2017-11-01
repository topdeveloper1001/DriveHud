//-----------------------------------------------------------------------
// <copyright file="TournamentInformation.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class TournamentInformation
    {
        public bool HasPassword { get; set; }

        public int EntranceFee { get; set; }

        public int Guaranteed { get; set; }

        public PrizeSummary PrizeSummary { get; set; }

        public string BrandName { get; set; }

        public int EventNumber { get; set; }

        public int AddOnAmount { get; set; }

        public int AddOnFee { get; set; }

        public int AddOnChips { get; set; }

        public int RebuyAmount { get; set; }

        public int RebuyFee { get; set; }

        public int RebuyCount { get; set; }

        public int RebuyPeriodLevel { get; set; }

        public int RebuyChips { get; set; }

        public LifeCycleData LifeCycleData { get; set; }

        public bool IsSatellite { get; set; }

        public IList<object> AwardEntries { get; set; }

        public int CurrentTotalPrizePool { get; set; }

        public BountyInformation BountyInformation { get; set; }

        public int CurrentRegularPrizePool { get; set; }

        public int CurrentBountyPrizePool { get; set; }

        public bool IsInTheMoney { get; set; }

        public bool IsCanceledByCRM { get; set; }

        public bool IsForceComplete { get; set; }

        public int AllowRebuyTypes { get; set; }

        public int AllowAddOnTypes { get; set; }

        public int BuyInGGPAmount { get; set; }

        public int ReBuyGGPAmount { get; set; }

        public int AddOnGGPAmount { get; set; }

        public int TourneyType { get; set; }

        public bool ChangeToRegularITM { get; set; }

        public bool EndTournamentITM { get; set; }

        public int PayOutType { get; set; }

        public int CurrentGGPPrizePool { get; set; }

        public int CashPrize { get; set; }

        public bool IsMainEvent { get; set; }

        public int FortuneBuyInAmount { get; set; }

        public int BuyInCookieCount { get; set; }

        public int ChipValuePerCookieCount { get; set; }

        public PvtInfo PvtInfo { get; set; }

        public RegisterPolicyInformation RegisterPolicyInformation { get; set; }

        public string Id { get; set; }

        public int BuyIn { get; set; }

        public string Name { get; set; }

        public int RegisteredPlayers { get; set; }

        public int StartPlayersCount { get; set; }

        public bool CanRegister { get; set; }

        public bool CanUnregister { get; set; }

        public bool CanStaking { get; set; }

        public bool OnlyTicket { get; set; }

        public Configuration Configuration { get; set; }

        public IList<int> TicketTemplateList { get; set; }

        public int Status { get; set; }

        public int AllowBuyInTypes { get; set; }
    }
}