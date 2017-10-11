//-----------------------------------------------------------------------
// <copyright file="NoteSettingsObject.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.ActionsObjects;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    public class NoteSettingsObject : ReactiveObject
    {
        public NoteSettingsObject()
        {
            Cash = true;
            Tournament = true;

            FacingUnopened = true;
            Facing2PlusLimpers = true;
            FacingRaisersCallers = true;
            Facing1Limper = true;
            Facing1Raiser = true;
            Facing2Raisers = true;

            TypeNoLimit = true;
            TypePotLimit = true;
            TypeLimit = true;
            PlayersNo34 = true;
            PlayersNo56 = true;
            PlayersNoHeadsUp = true;
            PlayersNoMax = true;
            PlayersNoMinVal = 2;
            PlayersNoMaxVal = 10;

            PositionBB = true;
            PositionButton = true;
            PositionCutoff = true;
            PositionEarly = true;
            PositionMiddle = true;
            PositionSB = true;

            ExcludedStakes = new List<Stake>();
            ExcludedCardsList = new List<string>();
            SelectedFilters = new List<FilterObject>();
            SelectedFiltersComparison = new List<FilterObject>();

            TagPlayer = new PlayerObject(PlayerTypeEnum.Tag);
            FishPlayer = new PlayerObject(PlayerTypeEnum.Fish);
            WhalePlayer = new PlayerObject(PlayerTypeEnum.Whale);
            GamblerPlayer = new PlayerObject(PlayerTypeEnum.Gambler);
            LagPlayer = new PlayerObject(PlayerTypeEnum.Lag);
            RockPlayer = new PlayerObject(PlayerTypeEnum.Rock);
            NitPlayer = new PlayerObject(PlayerTypeEnum.Nit);

            FlopHvSettings = new HandValueSettings();
            TurnHvSettings = new HandValueSettings();
            RiverHvSettings = new HandValueSettings();

            FlopTextureSettings = new FlopTextureSettings();
            TurnTextureSettings = new TurnTextureSettings();
            RiverTextureSettings = new RiverTextureSettings();

            FlopActions = new ActionSettings();
            TurnActions = new ActionSettings();
            RiverActions = new ActionSettings();
            PreflopActions = new ActionSettings();
        }

        public List<FilterObject> SelectedFilters { get; set; }
        public List<FilterObject> SelectedFiltersComparison { get; set; }

        public string ExcludedCards { get; set; }

        public List<Stake> ExcludedStakes { get; set; }

        [XmlIgnore]
        public List<string> ExcludedCardsList
        {
            get
            {
                return ExcludedCards.Contains(",")
                           ? new List<string>(ExcludedCards.Split(','))
                           : string.IsNullOrEmpty(ExcludedCards)
                                 ? new List<string>()
                                 : new List<string> { ExcludedCards };
            }
            set
            {
                ExcludedCards = string.Empty;

                foreach (string card in value)
                {
                    ExcludedCards += card + ',';
                }
                if (ExcludedCards.Contains(","))
                    ExcludedCards = ExcludedCards.Remove(ExcludedCards.LastIndexOf(','), 1);
            }
        }

        public bool TypeNoLimit { get; set; }
        public bool TypePotLimit { get; set; }
        public bool TypeLimit { get; set; }
        public bool Cash { get; set; }
        public bool Tournament { get; set; }

        #region Number of players

        private bool playersNoHeadsUp;

        public bool PlayersNoHeadsUp
        {
            get
            {
                return playersNoHeadsUp;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNoHeadsUp, value);
            }
        }

        private bool playersNo34;

        public bool PlayersNo34
        {
            get
            {
                return playersNo34;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNo34, value);
            }
        }

        private bool playersNo56;

        public bool PlayersNo56
        {
            get
            {
                return playersNo56;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNo56, value);
            }
        }

        private bool playersNoMax;

        public bool PlayersNoMax
        {
            get
            {
                return playersNoMax;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNoMax, value);
            }
        }

        private bool playersNoCustom;

        public bool PlayersNoCustom
        {
            get
            {
                return playersNoCustom;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNoCustom, value);
                PlayersNoHeadsUp = PlayersNo34 = PlayersNo56 = PlayersNoMax = !playersNoCustom;
            }
        }

        private int playersNoMinVal;

        public int PlayersNoMinVal
        {
            get
            {
                return playersNoMinVal;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNoMinVal, value);
            }
        }

        private int playersNoMaxVal;

        public int PlayersNoMaxVal
        {
            get
            {
                return playersNoMaxVal;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playersNoMaxVal, value);
            }
        }

        #endregion

        public bool PositionSB { get; set; }
        public bool PositionEarly { get; set; }
        public bool PositionCutoff { get; set; }
        public bool PositionBB { get; set; }
        public bool PositionMiddle { get; set; }
        public bool PositionButton { get; set; }

        public bool FacingUnopened { get; set; }
        public bool Facing2PlusLimpers { get; set; }
        public bool FacingRaisersCallers { get; set; }
        public bool Facing1Limper { get; set; }
        public bool Facing1Raiser { get; set; }
        public bool Facing2Raisers { get; set; }

        public bool PositionSBRaiser { get; set; }
        public bool PositionEarlyRaiser { get; set; }
        public bool PositionCutoffRaiser { get; set; }
        public bool PositionBBRaiser { get; set; }
        public bool PositionMiddleRaiser { get; set; }
        public bool PositionButtonRaiser { get; set; }

        public bool PositionSB3Bet { get; set; }
        public bool PositionEarly3Bet { get; set; }
        public bool PositionCutoff3Bet { get; set; }
        public bool PositionBB3Bet { get; set; }
        public bool PositionMiddle3Bet { get; set; }
        public bool PositionButton3Bet { get; set; }

        public double MBCMinSizeOfPot { get; set; }

        [XmlIgnore]
        public bool MBCWentToShowdown
        {
            get { return SelectedFilters.Any(p => p.Filter == FilterEnum.SawShowdown); }
        }

        [XmlIgnore]
        public bool MBCAllInPreFlop
        {
            get { return SelectedFilters.Any(p => p.Filter == FilterEnum.AllinPreflop); }
        }

        public PlayerObject TagPlayer { get; set; }
        public PlayerObject FishPlayer { get; set; }
        public PlayerObject WhalePlayer { get; set; }
        public PlayerObject GamblerPlayer { get; set; }
        public PlayerObject LagPlayer { get; set; }
        public PlayerObject RockPlayer { get; set; }
        public PlayerObject NitPlayer { get; set; }

        public ActionSettings PreflopActions { get; set; }
        public ActionSettings FlopActions { get; set; }
        public ActionSettings TurnActions { get; set; }
        public ActionSettings RiverActions { get; set; }

        public HandValueSettings FlopHvSettings { get; set; }
        public HandValueSettings RiverHvSettings { get; set; }
        public HandValueSettings TurnHvSettings { get; set; }

        public FlopTextureSettings FlopTextureSettings { get; set; }
        public TurnTextureSettings TurnTextureSettings { get; set; }
        public RiverTextureSettings RiverTextureSettings { get; set; }

        public override bool Equals(object x)
        {
            NoteSettingsObject x1 = (NoteSettingsObject)x;
            NoteSettingsObject x2 = this;

            return x1.MBCAllInPreFlop == x2.MBCAllInPreFlop &&
                   x1.MBCMinSizeOfPot == x2.MBCMinSizeOfPot &&
                   x1.MBCWentToShowdown == x2.MBCWentToShowdown &&
                   x1.PlayersNo34 == x2.PlayersNo34 &&
                   x1.PlayersNo56 == x2.PlayersNo56 &&
                   x1.PlayersNoCustom == x2.PlayersNoCustom &&
                   x1.PlayersNoHeadsUp == x2.PlayersNoHeadsUp &&
                   x1.PlayersNoMax == x2.PlayersNoMax &&
                   x1.PlayersNoMaxVal == x2.PlayersNoMaxVal &&
                   x1.PlayersNoMinVal == x2.PlayersNoMinVal &&
                   x1.PositionBB == x2.PositionBB &&
                   x1.PositionButton == x2.PositionButton &&
                   x1.PositionCutoff == x2.PositionCutoff &&
                   x1.PositionEarly == x2.PositionEarly &&
                   x1.PositionMiddle == x2.PositionMiddle &&
                   x1.PositionSB == x2.PositionSB &&
                   x1.TypeLimit == x2.TypeLimit &&
                   x1.TypeNoLimit == x2.TypeNoLimit &&
                   x1.TypePotLimit == x2.TypePotLimit && SelectedStakesEquality(x1.ExcludedStakes) &&
                   SelectedCardsEquality(x1.ExcludedCardsList) &&
                   x1.FishPlayer.Equals(x2.FishPlayer) &&
                   x1.GamblerPlayer.Equals(x2.GamblerPlayer) &&
                   x1.TagPlayer.Equals(x2.TagPlayer) &&
                   x1.LagPlayer.Equals(x2.LagPlayer) &&
                   x1.RockPlayer.Equals(x2.RockPlayer) &&
                   x1.NitPlayer.Equals(x2.NitPlayer) &&
                   x1.WhalePlayer.Equals(x2.WhalePlayer) &&
                   CompareSelectedFilters(x1.SelectedFilters) &&
                   x1.FlopHvSettings.Equals(x2.FlopHvSettings) &&
                   x1.TurnHvSettings.Equals(x2.TurnHvSettings) &&
                   x1.RiverHvSettings.Equals(x2.RiverHvSettings) &&
                   x1.FlopTextureSettings.Equals(x2.FlopTextureSettings) &&
                   x1.TurnTextureSettings.Equals(x2.TurnTextureSettings) &&
                   x1.RiverTextureSettings.Equals(x2.RiverTextureSettings) &&
                   x1.FlopActions.Equals(x2.FlopActions) &&
                   x1.TurnActions.Equals(x2.TurnActions) &&
                   x1.RiverActions.Equals(x2.RiverActions) &&
                   x1.PreflopActions.Equals(x2.PreflopActions) &&
                   x1.PositionBBRaiser == x2.PositionBBRaiser &&
                   x1.PositionButtonRaiser == x2.PositionButtonRaiser &&
                   x1.PositionCutoffRaiser == x2.PositionCutoffRaiser &&
                   x1.PositionEarlyRaiser == x2.PositionEarlyRaiser &&
                   x1.PositionMiddleRaiser == x2.PositionMiddleRaiser &&
                   x1.PositionSBRaiser == x2.PositionSBRaiser &&
                   x1.PositionBB3Bet == x2.PositionBB3Bet &&
                   x1.PositionButton3Bet == x2.PositionButton3Bet &&
                   x1.PositionCutoff3Bet == x2.PositionCutoff3Bet &&
                   x1.PositionEarly3Bet == x2.PositionEarly3Bet &&
                   x1.PositionMiddle3Bet == x2.PositionMiddle3Bet &&
                   x1.PositionSB3Bet == x2.PositionSB3Bet &&
                   x1.Facing1Limper == x2.Facing1Limper &&
                   x1.Facing1Raiser == x2.Facing1Raiser &&
                   x1.Facing2PlusLimpers == x2.Facing2PlusLimpers &&
                   x1.Facing2Raisers == x2.Facing2Raisers &&
                   x1.FacingRaisersCallers == x2.FacingRaisersCallers &&
                   x1.FacingUnopened == x2.FacingUnopened;
        }

        private bool CompareSelectedFilters(ICollection<FilterObject> newList)
        {
            if (newList.Count != SelectedFilters.Count)
                return false;

            foreach (FilterObject filter in newList)
            {
                FilterObject existingFilter = SelectedFilters.FirstOrDefault(p => p.Tag == filter.Tag);

                if (existingFilter == null)
                    return false;
                if (existingFilter.Value != filter.Value)
                    return false;
            }

            foreach (FilterObject filter in SelectedFilters)
            {
                FilterObject existingFilter = newList.FirstOrDefault(p => p.Tag == filter.Tag);

                if (existingFilter == null)
                    return false;
                if (existingFilter.Value != filter.Value)
                    return false;
            }

            return true;
        }

        private bool SelectedStakesEquality(List<Stake> newList)
        {
            if (newList.Count != ExcludedStakes.Count)
                return false;

            foreach (Stake stake in newList)
            {
                if (ExcludedStakes.FindAll(p => p.Name == stake.Name).Count == 0)
                    return false;
            }

            foreach (Stake stake in ExcludedStakes)
            {
                if (newList.FindAll(p => p.Name == stake.Name).Count == 0)
                    return false;
            }

            return true;
        }

        private bool SelectedCardsEquality(ICollection<string> newList)
        {
            if (newList.Count != ExcludedCardsList.Count)
                return false;

            foreach (string card in newList)
            {
                if (!ExcludedCardsList.Contains(card))
                    return false;
            }

            foreach (string card in ExcludedCardsList)
            {
                if (!newList.Contains(card))
                    return false;
            }

            return true;
        }

        public List<long> GetPreflopFacingValues(ClientType client)
        {
            List<long> result = new List<long>();

            if (client == ClientType.HoldemManager)
            {
                if (Facing1Limper)
                    result.Add(1);
                if (FacingUnopened)
                    result.Add(0);
                if (Facing2PlusLimpers)
                    result.Add(2);
                if (Facing1Raiser)
                    result.Add(3);
                if (FacingRaisersCallers)
                    result.Add(4);
                if (Facing2Raisers)
                    result.Add(5);
            }

            return result;
        }
    }
}