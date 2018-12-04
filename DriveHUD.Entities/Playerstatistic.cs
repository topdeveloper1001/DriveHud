//-----------------------------------------------------------------------
// <copyright file="Playerstatistic.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    [ProtoContract]
    public partial class Playerstatistic : INotifyPropertyChanged
    {
#if DEBUG
        public override string ToString()
        {
            return $"Player: {PlayerName}; TotalHands: {Totalhands}; Cards: {Cards}; VPIP: {Vpiphands}; NetWon: {NetWon}; Time: {Time}; GameNumber: {GameNumber}; Site: {PokersiteId}";
        }
#endif

        public virtual int CompiledplayerresultsId { get; set; }
        [Required, ProtoMember(232)]
        public virtual int PlayerId { get; set; }
        [Required, ProtoMember(1)]
        public virtual int Wonhand { get; set; }
        [Required, ProtoMember(2)]
        public virtual int Totalpostflopstreetsplayed { get; set; }
        [Required, ProtoMember(3)]
        public virtual int Bigblindstealdefended { get; set; }
        [Required, ProtoMember(4)]
        public virtual int Calledthreebetpreflop { get; set; }
        [Required, ProtoMember(5)]
        public virtual int Turnfoldippassonflopcb { get; set; }
        [Required, ProtoMember(6)]
        public virtual int Rivercallippassonturncb { get; set; }
        [Required, ProtoMember(7)]
        public virtual int Playedyearandmonth { get; set; }
        [Required, ProtoMember(8)]
        public virtual int Turncontinuationbetmade { get; set; }
        [Required, ProtoMember(9)]
        public virtual int Wonhandwhensawturn { get; set; }
        [Required, ProtoMember(10)]
        public virtual int Turncontinuationbetpossible { get; set; }
        [Required, ProtoMember(11)]
        public virtual int Couldcoldcall { get; set; }
        [Required, ProtoMember(12)]
        public virtual int Totalamountwonincents { get; set; }
        [Required, ProtoMember(13)]
        public virtual int Facedfourbetpreflop { get; set; }
        [Required, ProtoMember(14)]
        public virtual int Flopcontinuationbetpossible { get; set; }
        [Required, ProtoMember(15)]
        public virtual int Riverfoldippassonturncb { get; set; }
        [Required, ProtoMember(16)]
        public virtual int Facingturncontinuationbet { get; set; }
        [Required, ProtoMember(17)]
        public virtual int Sawlargeshowdown { get; set; }
        [Required, ProtoMember(18)]
        public virtual int Raisedthreebetpreflop { get; set; }
        [Required, ProtoMember(19)]
        public virtual int Vpiphands { get; set; }
        [Required, ProtoMember(20)]
        public virtual int Facingrivercontinuationbet { get; set; }
        [Required, ProtoMember(21)]
        public virtual int Wonnonsmallshowdown { get; set; }
        [Required, ProtoMember(22)]
        public virtual int Totalbets { get; set; }
        [Required, ProtoMember(23)]
        public virtual int Riverraiseippassonturncb { get; set; }
        [Required, ProtoMember(24)]
        public virtual short Numberofplayers { get; set; }
        [Required, ProtoMember(25)]
        public virtual int Pfrhands { get; set; }
        [Required, ProtoMember(26)]
        public virtual int Smallblindstealdefended { get; set; }
        [Required, ProtoMember(27)]
        public virtual int Wonhandwhensawriver { get; set; }
        [Required, ProtoMember(28)]
        public virtual int Foldedtoflopcontinuationbet { get; set; }
        [Required, ProtoMember(29)]
        public virtual int Wonnonsmallshowdownlimpedflop { get; set; }
        [Required, ProtoMember(30)]
        public virtual int Calledfourbetpreflop { get; set; }
        [Required, ProtoMember(31)]
        public virtual int Rivercontinuationbetmade { get; set; }
        [Required, ProtoMember(32)]
        public virtual int Calledturncontinuationbet { get; set; }
        [Required, ProtoMember(33)]
        public virtual int Bigblindstealreraised { get; set; }
        [Required, ProtoMember(34)]
        public virtual int Totalcalls { get; set; }
        [Required, ProtoMember(35)]
        public virtual int Sawshowdown { get; set; }
        [Required, ProtoMember(36)]
        public virtual int Calledflopcontinuationbet { get; set; }
        [Required, ProtoMember(37)]
        public virtual short BbgroupId { get; set; }
        [Required, ProtoMember(38)]
        public virtual int Bigblindstealfaced { get; set; }
        [Required, ProtoMember(39)]
        public virtual int Turnraiseippassonflopcb { get; set; }
        [Required, ProtoMember(40)]
        public virtual int Turncallippassonflopcb { get; set; }
        [Required, ProtoMember(41)]
        public virtual int Wonlargeshowdown { get; set; }
        [Required, ProtoMember(42)]
        public virtual int Couldthreebet { get; set; }
        [Required, ProtoMember(43)]
        public virtual int Smallblindstealfaced { get; set; }
        [Required, ProtoMember(44)]
        public virtual int Sawnonsmallshowdown { get; set; }
        [Required, ProtoMember(45)]
        public virtual int Foldedtothreebetpreflop { get; set; }
        [Required, ProtoMember(46)]
        public virtual int Foldedtorivercontinuationbet { get; set; }
        [Required, ProtoMember(47)]
        public virtual int Raisedturncontinuationbet { get; set; }
        [Required, ProtoMember(48)]
        public virtual int Smallblindstealreraised { get; set; }
        [Required, ProtoMember(49)]
        public virtual int Wonshowdown { get; set; }
        [Required, ProtoMember(50)]
        public virtual int Raisedflopcontinuationbet { get; set; }
        [Required, ProtoMember(51)]
        public virtual int Wonhandwhensawflop { get; set; }
        [Required, ProtoMember(52)]
        public virtual int Flopcontinuationbetmade { get; set; }
        [Required, ProtoMember(53)]
        public virtual int Foldedtoturncontinuationbet { get; set; }
        [Required, ProtoMember(54)]
        public virtual int Calledtwopreflopraisers { get; set; }
        [Required, ProtoMember(55)]
        public virtual int Raisedtwopreflopraisers { get; set; }
        [Required, ProtoMember(56)]
        public virtual int Totalbbswon { get; set; }
        [Required, ProtoMember(57)]
        public virtual short GametypeId { get; set; }
        [Required, ProtoMember(58)]
        public virtual int Totalrakeincents { get; set; }
        [Required, ProtoMember(59)]
        public virtual int Couldsqueeze { get; set; }
        [Required, ProtoMember(60)]
        public virtual int Foldedtofourbetpreflop { get; set; }
        [Required, ProtoMember(61)]
        public virtual int Totalhands { get; set; }
        [Required, ProtoMember(62)]
        public virtual int Facedthreebetpreflop { get; set; }
        [Required, ProtoMember(63)]
        public virtual int Facingflopcontinuationbet { get; set; }
        [Required, ProtoMember(64)]
        public virtual int Rivercontinuationbetpossible { get; set; }
        [Required, ProtoMember(65)]
        public virtual int Didcoldcall { get; set; }
        [Required, ProtoMember(66)]
        public virtual int Wonlargeshowdownlimpedflop { get; set; }
        [Required, ProtoMember(67)]
        public virtual int Sawflop { get; set; }
        [Required, ProtoMember(68)]
        public virtual int Totalaggressivepostflopstreetsseen { get; set; }
        [Required, ProtoMember(69)]
        public virtual int Didthreebet { get; set; }
        [Required, ProtoMember(70)]
        public virtual int Raisedfourbetpreflop { get; set; }
        [Required, ProtoMember(71)]
        public virtual int Sawlargeshowdownlimpedflop { get; set; }
        [Required, ProtoMember(72)]
        public virtual int Raisedrivercontinuationbet { get; set; }
        [Required, ProtoMember(73)]
        public virtual int Facingtwopreflopraisers { get; set; }
        [Required, ProtoMember(74)]
        public virtual int Sawnonsmallshowdownlimpedflop { get; set; }
        [Required, ProtoMember(75)]
        public virtual int Didsqueeze { get; set; }

        [Required, ProtoMember(76)]
        public virtual int Calledrivercontinuationbet { get; set; }

        [ProtoMember(77)]
        public virtual string PlayerName { get; set; }

        [ProtoMember(78)]
        public virtual bool IsSmallBlind { get; set; }
        [ProtoMember(79)]
        public virtual bool IsBigBlind { get; set; }
        [ProtoMember(80)]
        public virtual bool IsDealer { get; set; }
        [ProtoMember(81)]
        public virtual bool IsCutoff { get; set; }
        [ProtoMember(82)]
        public virtual string PositionString { get; set; }
        [ProtoMember(83)]
        public virtual DateTime Time { get; set; }
        [ProtoMember(84)]
        public virtual string Cards { get; set; }
        [ProtoMember(85)]
        public virtual string Line { get; set; }
        [ProtoMember(86)]
        public virtual string Board { get; set; }
        [ProtoMember(87)]
        public virtual EnumFacingPreflop FacingPreflop { get; set; }
        [ProtoMember(88)]
        public virtual string Action { get; set; }
        [ProtoMember(89)]
        public virtual string Allin { get; set; }
        [ProtoMember(90)]
        public virtual decimal Equity { get; set; }
        [ProtoMember(91)]
        public virtual decimal EquityDiff { get; set; }

        [ProtoMember(92)]
        public virtual string GameType { get; set; }

        [ProtoMember(93)]
        public virtual string Stakes { get; set; }

        [ProtoMember(94)]
        public virtual int SawUnopenedPot { get; set; }

        [ProtoMember(95)]
        public virtual int UO_PFR_EP { get; set; }

        [ProtoMember(96)]
        public virtual int UO_PFR_MP { get; set; }

        [ProtoMember(97)]
        public virtual int UO_PFR_CO { get; set; }

        [ProtoMember(98)]
        public virtual int UO_PFR_BN { get; set; }

        [ProtoMember(99)]
        public virtual int UO_PFR_SB { get; set; }

        [ProtoMember(100)]
        public virtual int UO_PFR_BB { get; set; }

        [ProtoMember(101)]
        public virtual int FirstRaiser { get; set; }

        [ProtoMember(102)]
        public virtual int StealPossible { get; set; }

        [ProtoMember(103)]
        public virtual int StealMade { get; set; }

        [ProtoMember(104)]
        public virtual int Couldfourbet { get; set; }

        [ProtoMember(105)]
        public virtual int Didfourbet { get; set; }

        [ProtoMember(107)]
        public virtual int Bigblindstealfolded { get; set; }

        [ProtoMember(108)]
        public virtual int Smallblindstealfolded { get; set; }

        [ProtoMember(109)]
        public virtual long GameNumber { get; set; }

        [ProtoMember(110)]
        public virtual decimal Pot { get; set; }

        [ProtoMember(111)]
        public virtual bool IsTourney { get; set; }

        [ProtoMember(112)]
        public virtual string TournamentId { get; set; }

        [ProtoMember(113)]
        public virtual string TableType { get; set; }

        [ProtoMember(114)]
        public virtual decimal SmallBlind { get; set; }

        [ProtoMember(115)]
        public virtual decimal BigBlind { get; set; }

        [ProtoMember(116)]
        public virtual int PokersiteId { get; set; }

        [ProtoMember(117)]
        public virtual decimal StartingStack { get; set; }

        [ProtoMember(118)]
        public virtual decimal Ante { get; set; }

        [ProtoMember(119)]
        public virtual int SawTurn { get; set; }

        [ProtoMember(120)]
        public virtual int SawRiver { get; set; }

        [ProtoMember(121)]
        public virtual int DidOpenRaise { get; set; }

        [ProtoMember(122)]
        public virtual int DidCheckRaise { get; set; }

        [ProtoMember(123)]
        public virtual bool IsRelativePosition { get; set; }

        [ProtoMember(124)]
        public virtual int IsRaisedLimpers { get; set; }

        [ProtoMember(125)]
        public virtual bool IsRelative3BetPosition { get; set; }

        [ProtoMember(126)]
        public virtual int CurrencyId { get; set; }

        [ProtoMember(127)]
        public virtual EnumPosition Position { get; set; }

        [ProtoMember(128)]
        public virtual short PokergametypeId { get; set; }

        [ProtoMember(131)]
        public virtual int DidThreeBetInMp { get; set; }

        [ProtoMember(132)]
        public virtual int DidThreeBetInCo { get; set; }

        [ProtoMember(133)]
        public virtual int DidThreeBetInBtn { get; set; }

        [ProtoMember(134)]
        public virtual int DidThreeBetInSb { get; set; }

        [ProtoMember(135)]
        public virtual int DidThreeBetInBb { get; set; }

        [ProtoMember(136)]
        public virtual int DidFourBetInMp { get; set; }

        [ProtoMember(137)]
        public virtual int DidFourBetInCo { get; set; }

        [ProtoMember(138)]
        public virtual int DidFourBetInBtn { get; set; }

        [ProtoMember(139)]
        public virtual int DidFourBetInSb { get; set; }

        [ProtoMember(140)]
        public virtual int DidFourBetInBb { get; set; }

        [ProtoMember(148)]
        public virtual int DidDonkBet { get; set; }

        [ProtoMember(149)]
        public virtual int CouldDonkBet { get; set; }

        [ProtoMember(150)]
        public virtual int DidDelayedTurnCBet { get; set; }

        [ProtoMember(151)]
        public virtual int CouldDelayedTurnCBet { get; set; }

        [ProtoMember(152)]
        public virtual int DidFlopCheckRaise { get; set; }

        [ProtoMember(153)]
        public virtual int DidTurnCheckRaise { get; set; }

        [ProtoMember(154)]
        public virtual int DidRiverCheckRaise { get; set; }

        [ProtoMember(155)]
        public virtual int PlayedFloatFlop { get; set; }

        [ProtoMember(156)]
        public virtual int CouldRaiseFlop { get; set; }

        [ProtoMember(157)]
        public virtual int CouldRaiseTurn { get; set; }

        [ProtoMember(158)]
        public virtual int CouldRaiseRiver { get; set; }

        [ProtoMember(159)]
        public virtual int TurnContinuationBetWithAirMade { get; set; }

        [ProtoMember(160)]
        public virtual int WasFlop { get; set; }

        [ProtoMember(161)]
        public virtual int WasTurn { get; set; }

        [ProtoMember(162)]
        public virtual int WasRiver { get; set; }

        [ProtoMember(163)]
        public virtual int DidRaiseFlop { get; set; }

        [ProtoMember(164)]
        public virtual int DidRaiseTurn { get; set; }

        [ProtoMember(165)]
        public virtual int DidRaiseRiver { get; set; }

        [ProtoMember(166)]
        public virtual int CouldThreeBetVsSteal { get; set; }

        [ProtoMember(167)]
        public virtual int DidThreeBetVsSteal { get; set; }

        [ProtoMember(168)]
        public virtual int CouldCheckRiverOnBXLine { get; set; }

        [ProtoMember(169)]
        public virtual int DidCheckRiverOnBXLine { get; set; }

        [ProtoMember(170)]
        public virtual int TotalAggressiveBets { get; set; }

        [ProtoMember(171)]
        public virtual int DidColdCallIp { get; set; }

        [ProtoMember(172)]
        public virtual int DidColdCallOop { get; set; }

        [ProtoMember(173)]
        public virtual int DidthreebetBluffInSb { get; set; }

        [ProtoMember(174)]
        public virtual int DidthreebetBluffInBb { get; set; }

        [ProtoMember(175)]
        public virtual int DidthreebetBluffInBlinds { get; set; }

        [ProtoMember(176)]
        public virtual int DidfourbetBluff { get; set; }

        [ProtoMember(177)]
        public virtual int DidFourBetBluffInBtn { get; set; }

        [ProtoMember(178)]
        public virtual int DidBluffedRiver { get; set; }

        [ProtoMember(179)]
        public virtual int BlindsStealDefended { get; set; }

        [ProtoMember(180)]
        public virtual int DidCheckFlop { get; set; }

        [ProtoMember(181)]
        public virtual int FacedHandsUpOnFlop { get; set; }

        [ProtoMember(182)]
        public virtual int FacedMultiWayOnFlop { get; set; }

        public virtual string SessionCode { get; set; }

        [ProtoMember(184)]
        public virtual int LimpPossible { get; set; }

        [ProtoMember(185)]
        public virtual int LimpMade { get; set; }

        [ProtoMember(186)]
        public virtual int LimpFaced { get; set; }

        [ProtoMember(187)]
        public virtual int LimpCalled { get; set; }

        [ProtoMember(188)]
        public virtual int LimpFolded { get; set; }

        [ProtoMember(189)]
        public virtual int LimpReraised { get; set; }

        [ProtoMember(190)]
        public virtual decimal MRatio { get; set; }

        [ProtoMember(191)]
        public virtual decimal StackInBBs { get; set; }

        [ProtoMember(192)]
        public virtual int TotalbetsFlop { get; set; }

        [ProtoMember(193)]
        public virtual int TotalbetsTurn { get; set; }

        [ProtoMember(194)]
        public virtual int TotalbetsRiver { get; set; }

        [ProtoMember(195)]
        public virtual int TotalcallsFlop { get; set; }

        [ProtoMember(196)]
        public virtual int TotalcallsTurn { get; set; }

        [ProtoMember(197)]
        public virtual int TotalcallsRiver { get; set; }

        // Since aggression by street was added later need  separate SawStreet stats for it 
        [ProtoMember(198)]
        public virtual int FlopAggPossible { get; set; }

        [ProtoMember(199)]
        public virtual int TurnAggPossible { get; set; }

        [ProtoMember(200)]
        public virtual int RiverAggPossible { get; set; }

        [ProtoMember(201)]
        public virtual int Smallblindstealattempted { get; set; }

        [ProtoMember(202)]
        public virtual int FlopContinuationBetInThreeBetPotPossible { get; set; }

        [ProtoMember(203)]
        public virtual int FlopContinuationBetInThreeBetPotMade { get; set; }

        [ProtoMember(204)]
        public virtual int FlopContinuationBetInFourBetPotPossible { get; set; }

        [ProtoMember(205)]
        public virtual int FlopContinuationBetInFourBetPotMade { get; set; }

        [ProtoMember(206)]
        public virtual int FlopContinuationBetVsOneOpponentPossible { get; set; }

        [ProtoMember(207)]
        public virtual int FlopContinuationBetVsOneOpponentMade { get; set; }

        [ProtoMember(208)]
        public virtual int FlopContinuationBetVsTwoOpponentsPossible { get; set; }

        [ProtoMember(209)]
        public virtual int FlopContinuationBetVsTwoOpponentsMade { get; set; }

        [ProtoMember(210)]
        public virtual int MultiWayFlopContinuationBetPossible { get; set; }

        [ProtoMember(211)]
        public virtual int MultiWayFlopContinuationBetMade { get; set; }

        [ProtoMember(212)]
        public virtual int FlopContinuationBetMonotonePotPossible { get; set; }

        [ProtoMember(213)]
        public virtual int FlopContinuationBetMonotonePotMade { get; set; }

        [ProtoMember(214)]
        public virtual int FlopContinuationBetRagPotPossible { get; set; }

        [ProtoMember(215)]
        public virtual int FlopContinuationBetRagPotMade { get; set; }

        [ProtoMember(216)]
        public virtual int FacingFlopContinuationBetFromThreeBetPot { get; set; }

        [ProtoMember(217)]
        public virtual int FoldedToFlopContinuationBetFromThreeBetPot { get; set; }

        [ProtoMember(218)]
        public virtual int CalledFlopContinuationBetFromThreeBetPot { get; set; }

        [ProtoMember(219)]
        public virtual int RaisedFlopContinuationBetFromThreeBetPot { get; set; }

        [ProtoMember(220)]
        public virtual int FacingFlopContinuationBetFromFourBetPot { get; set; }

        [ProtoMember(221)]
        public virtual int FoldedToFlopContinuationBetFromFourBetPot { get; set; }

        [ProtoMember(222)]
        public virtual int CalledFlopContinuationBetFromFourBetPot { get; set; }

        [ProtoMember(223)]
        public virtual int RaisedFlopContinuationBetFromFourBetPot { get; set; }

        [ProtoMember(224)]
        public virtual int DidThreeBetIp { get; set; }

        [ProtoMember(225)]
        public virtual int CouldThreeBetIp { get; set; }

        [ProtoMember(226)]
        public virtual int DidThreeBetOop { get; set; }

        [ProtoMember(227)]
        public virtual int CouldThreeBetOop { get; set; }

        [ProtoMember(228)]
        public virtual int Flopcontinuationipbetmade { get; set; }

        [ProtoMember(229)]
        public virtual int Flopcontinuationipbetpossible { get; set; }

        [ProtoMember(230)]
        public virtual int Flopcontinuationoopbetmade { get; set; }

        [ProtoMember(231)]
        public virtual int Flopcontinuationoopbetpossible { get; set; }

        [ProtoMember(233)]
        public virtual int CheckFoldFlopPfrOop { get; set; }

        [ProtoMember(234)]
        public virtual int CheckFoldFlop3BetOop { get; set; }

        [ProtoMember(235)]
        public virtual int BetFoldFlopPfrRaiser { get; set; }

        [ProtoMember(236)]
        public virtual int BetFlopCalled3BetPreflopIp { get; set; }

        [ProtoMember(237)]
        public virtual int PfrOop { get; set; }

        [ProtoMember(238)]
        public virtual int PfrInEp { get; set; }

        [ProtoMember(239)]
        public virtual int PfrInMp { get; set; }

        [ProtoMember(240)]
        public virtual int PfrInCo { get; set; }

        [ProtoMember(241)]
        public virtual int PfrInBtn { get; set; }

        [ProtoMember(242)]
        public virtual int PfrInSb { get; set; }

        [ProtoMember(243)]
        public virtual int PfrInBb { get; set; }

        [ProtoMember(244)]
        public virtual int Buttonstealfaced { get; set; }

        [ProtoMember(245)]
        public virtual int Buttonstealdefended { get; set; }

        [ProtoMember(246)]
        public virtual int Buttonstealfolded { get; set; }

        [ProtoMember(247)]
        public virtual int Buttonstealreraised { get; set; }

        [ProtoMember(248)]
        public virtual decimal EVDiff { get; set; }

        [ProtoMember(249)]
        public virtual int FacedRaiseFlop { get; set; }

        [ProtoMember(250)]
        public virtual int FoldedFacedRaiseFlop { get; set; }

        [ProtoMember(251)]
        public virtual int CalledFacedRaiseFlop { get; set; }

        [ProtoMember(252)]
        public virtual int ReraisedFacedRaiseFlop { get; set; }

        [ProtoMember(253)]
        public virtual int FacedRaiseTurn { get; set; }

        [ProtoMember(254)]
        public virtual int FoldedFacedRaiseTurn { get; set; }

        [ProtoMember(255)]
        public virtual int CalledFacedRaiseTurn { get; set; }

        [ProtoMember(256)]
        public virtual int ReraisedFacedRaiseTurn { get; set; }

        [ProtoMember(257)]
        public virtual int FacedRaiseRiver { get; set; }

        [ProtoMember(258)]
        public virtual int FoldedFacedRaiseRiver { get; set; }

        [ProtoMember(259)]
        public virtual int CalledFacedRaiseRiver { get; set; }

        [ProtoMember(260)]
        public virtual int ReraisedFacedRaiseRiver { get; set; }

        [ProtoMember(261)]
        public virtual int CanBetWhenCheckedToFlop { get; set; }

        [ProtoMember(262)]
        public virtual int DidBetWhenCheckedToFlop { get; set; }

        [ProtoMember(263)]
        public virtual int CanBetWhenCheckedToTurn { get; set; }

        [ProtoMember(264)]
        public virtual int DidBetWhenCheckedToTurn { get; set; }

        [ProtoMember(265)]
        public virtual int CanBetWhenCheckedToRiver { get; set; }

        [ProtoMember(266)]
        public virtual int DidBetWhenCheckedToRiver { get; set; }

        [ProtoMember(267)]
        public virtual int FacedSqueez { get; set; }

        [ProtoMember(268)]
        public virtual int FoldedFacedSqueez { get; set; }

        [ProtoMember(269)]
        public virtual int CalledFacedSqueez { get; set; }

        [ProtoMember(270)]
        public virtual int ReraisedFacedSqueez { get; set; }

        [ProtoMember(271)]
        public virtual int LimpBtn { get; set; }

        [ProtoMember(272)]
        public virtual int LimpEp { get; set; }

        [ProtoMember(273)]
        public virtual int LimpMp { get; set; }

        [ProtoMember(274)]
        public virtual int LimpCo { get; set; }

        [ProtoMember(275)]
        public virtual int LimpSb { get; set; }

        [ProtoMember(141)]
        public virtual int DidColdCallInMp { get; set; }

        [ProtoMember(142)]
        public virtual int DidColdCallInCo { get; set; }

        [ProtoMember(143)]
        public virtual int DidColdCallInBtn { get; set; }

        [ProtoMember(144)]
        public virtual int DidColdCallInSb { get; set; }

        [ProtoMember(145)]
        public virtual int DidColdCallInBb { get; set; }

        [ProtoMember(276)]
        public virtual int DidColdCallInEp { get; set; }

        [ProtoMember(277)]
        public virtual int DidColdCallThreeBet { get; set; }

        [ProtoMember(278)]
        public virtual int CouldColdCallThreeBet { get; set; }

        [ProtoMember(279)]
        public virtual int DidColdCallFourBet { get; set; }

        [ProtoMember(280)]
        public virtual int CouldColdCallFourBet { get; set; }

        [ProtoMember(281)]
        public virtual int DidColdCallVsOpenRaiseBtn { get; set; }

        [ProtoMember(282)]
        public virtual int DidColdCallVsOpenRaiseCo { get; set; }

        [ProtoMember(283)]
        public virtual int DidColdCallVsOpenRaiseSb { get; set; }

        [ProtoMember(284)]
        public virtual int CouldColdCallVsOpenRaiseBtn { get; set; }

        [ProtoMember(285)]
        public virtual int CouldColdCallVsOpenRaiseCo { get; set; }

        [ProtoMember(286)]
        public virtual int CouldColdCallVsOpenRaiseSb { get; set; }

        [ProtoMember(287)]
        public virtual int PreflopIP { get; set; }

        [ProtoMember(288)]
        public virtual int PreflopOOP { get; set; }

        [ProtoMember(289)]
        public virtual int NumberOfWalks { get; set; }

        [ProtoMember(290)]
        public virtual int CouldFlopCheckRaise { get; set; }

        [ProtoMember(291)]
        public virtual int CouldTurnCheckRaise { get; set; }

        [ProtoMember(292)]
        public virtual int CouldRiverCheckRaise { get; set; }

        [ProtoMember(293)]
        public virtual int MaxPlayers { get; set; }

        [ProtoMember(294)]
        public virtual int CouldBetFlopCalled3BetPreflopIp { get; set; }

        [ProtoMember(295)]
        public virtual int CouldBetFoldFlopPfrRaiser { get; set; }

        [ProtoMember(296)]
        public virtual int FacedFlopCheckRaise { get; set; }

        [ProtoMember(297)]
        public virtual int FacedTurnCheckRaise { get; set; }

        [ProtoMember(298)]
        public virtual int FacedRiverCheckRaise { get; set; }

        [ProtoMember(299)]
        public virtual int FoldedToFlopCheckRaise { get; set; }

        [ProtoMember(300)]
        public virtual int FoldedToTurnCheckRaise { get; set; }

        [ProtoMember(301)]
        public virtual int FoldedToRiverCheckRaise { get; set; }

        [ProtoMember(302)]
        public virtual int CalledTurnCheckRaise { get; set; }

        [ProtoMember(303)]
        public virtual int CheckedRiverAfterBBLine { get; set; }

        [ProtoMember(304)]
        public virtual int CouldCheckRiverAfterBBLine { get; set; }

        [ProtoMember(305)]
        public virtual int DidBetRiverOnBXLine { get; set; }

        [ProtoMember(306)]
        public virtual int CouldBetRiverOnBXLine { get; set; }

        [ProtoMember(307)]
        public virtual int FacingflopcontinuationbetIP { get; set; }

        [ProtoMember(308)]
        public virtual int FacingflopcontinuationbetOOP { get; set; }

        [ProtoMember(309)]
        public virtual int CalledflopcontinuationbetIP { get; set; }

        [ProtoMember(310)]
        public virtual int CalledflopcontinuationbetOOP { get; set; }

        [ProtoMember(311)]
        public virtual int FoldToFlopcontinuationbetIP { get; set; }

        [ProtoMember(312)]
        public virtual int FoldToFlopcontinuationbetOOP { get; set; }

        [ProtoMember(313)]
        public virtual int FoldToThreeBetIP { get; set; }

        [ProtoMember(314)]
        public virtual int FoldToThreeBetOOP { get; set; }

        [ProtoMember(315)]
        public virtual int DidRiverBet { get; set; }

        [ProtoMember(316)]
        public virtual int CouldRiverBet { get; set; }

        [ProtoMember(317)]
        public virtual int Did5Bet { get; set; }

        [ProtoMember(318)]
        public virtual int Could5Bet { get; set; }

        [ProtoMember(319)]
        public virtual int CalledCheckRaiseVsFlopCBet { get; set; }

        [ProtoMember(320)]
        public virtual int FoldedCheckRaiseVsFlopCBet { get; set; }

        [ProtoMember(321)]
        public virtual int FacedCheckRaiseVsFlopCBet { get; set; }

        [ProtoMember(322)]
        public virtual int FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP { get; set; }

        [ProtoMember(323)]
        public virtual int CheckedCalledTurnWhenCheckedFlopAsPfr { get; set; }

        [ProtoMember(324)]
        public virtual int CheckedFoldedToTurnWhenCheckedFlopAsPfr { get; set; }

        [ProtoMember(325)]
        public virtual int FacedTurnBetWhenCheckedFlopAsPfr { get; set; }

        [ProtoMember(326)]
        public virtual int CalledTurnBetWhenCheckedFlopAsPfr { get; set; }

        [ProtoMember(327)]
        public virtual int FoldedToTurnBetWhenCheckedFlopAsPfr { get; set; }

        [ProtoMember(328)]
        public virtual int RaisedTurnBetWhenCheckedFlopAsPfr { get; set; }

        [ProtoMember(329)]
        public virtual decimal FlopBetToPotRatio { get; set; }

        [ProtoMember(330)]
        public virtual decimal TurnBetToPotRatio { get; set; }

        [ProtoMember(331)]
        public virtual int DidDelayedTurnCBetIn3BetPot { get; set; }

        [ProtoMember(332)]
        public virtual int CouldDelayedTurnCBetIn3BetPot { get; set; }

        [ProtoMember(333)]
        public virtual int DidFlopCheckBehind { get; set; }

        [ProtoMember(334)]
        public virtual int CouldFlopCheckBehind { get; set; }

        [ProtoMember(335)]
        public virtual int FacedDonkBet { get; set; }

        [ProtoMember(336)]
        public virtual int FoldedToDonkBet { get; set; }

        [ProtoMember(337)]
        public virtual int FoldedTurn { get; set; }

        [ProtoMember(338)]
        public virtual int FacedBetOnTurn { get; set; }

        [ProtoMember(339)]
        public virtual int CheckedCalledRiver { get; set; }

        [ProtoMember(340)]
        public virtual int CheckedFoldedRiver { get; set; }

        [ProtoMember(341)]
        public virtual int CheckedThenFacedBetOnRiver { get; set; }

        [ProtoMember(342)]
        public virtual decimal RiverBetToPotRatio { get; set; }

        [ProtoMember(343)]
        public virtual decimal RiverWonOnFacingBet { get; set; }

        [ProtoMember(344)]
        public virtual decimal RiverCallSizeOnFacingBet { get; set; }

        [ProtoMember(345)]
        public virtual int Faced5Bet { get; set; }

        [ProtoMember(346)]
        public virtual int FoldedTo5Bet { get; set; }

        [ProtoMember(347)]
        public virtual int ShovedFlopAfter4Bet { get; set; }

        [ProtoMember(348)]
        public virtual int CouldShoveFlopAfter4Bet { get; set; }

        [ProtoMember(349)]
        public virtual int FacedThreeBetIP { get; set; }

        [ProtoMember(350)]
        public virtual int FacedThreeBetOOP { get; set; }

        [ProtoMember(351)]
        public virtual int BetFlopWhenCheckedToSRP { get; set; }

        [ProtoMember(352)]
        public virtual int CouldBetFlopWhenCheckedToSRP { get; set; }

        [ProtoMember(353)]
        public virtual bool IsStraddle { get; set; }

        [ProtoMember(354)]
        public virtual uint TableTypeDescription { get; set; }

        [ProtoMember(355)]
        public virtual int CouldTurnBet { get; set; }

        [ProtoMember(356)]
        public virtual int CouldFlopBet { get; set; }

        [ProtoMember(357)]
        public virtual int FoldedFlop { get; set; }

        [ProtoMember(358)]
        public virtual int FacedBetOnFlop { get; set; }

        [ProtoMember(359)]
        public virtual int FacedBetOnRiver { get; set; }

        [ProtoMember(360)]
        public virtual int RiverVsBetFold { get; set; }

        [ProtoMember(361)]
        public virtual int FlopCBetSuccess { get; set; }

        [ProtoMember(362)]
        public virtual int DidCheckTurn { get; set; }

        [ProtoMember(363)]
        public virtual int TotalCallAmountOnRiver { get; set; }

        [ProtoMember(364)]
        public virtual int TotalWonAmountOnRiverCall { get; set; }

        [ProtoMember(365)]
        public virtual EnumPosition FirstRaiserPosition { get; set; } = EnumPosition.Undefined;

        [ProtoMember(366)]
        public virtual EnumPosition ThreeBettorPosition { get; set; } = EnumPosition.Undefined;

        [ProtoMember(367)]
        public virtual int CouldProbeBetTurn { get; set; }

        [ProtoMember(368)]
        public virtual int CouldProbeBetRiver { get; set; }

        [ProtoMember(369)]
        public virtual string PreflopActions { get; set; } = string.Empty;

        [ProtoMember(370)]
        public virtual string FlopActions { get; set; } = string.Empty;

        [ProtoMember(371)]
        public virtual string TurnActions { get; set; } = string.Empty;

        [ProtoMember(372)]
        public virtual string RiverActions { get; set; } = string.Empty;

        [ProtoMember(373)]
        public virtual int PreflopRaisersCount { get; set; }

        [ProtoMember(374)]
        public virtual decimal OpenRaisePreflopInBBs { get; set; }

        [ProtoMember(375)]
        public virtual int FacedDelayedCBet { get; set; }

        [ProtoMember(376)]
        public virtual int BetAmountPreflopInCents { get; set; }

        [ProtoMember(377)]
        public virtual int CallAmountPreflopInCents { get; set; }

        [ProtoMember(378)]
        public virtual int PostAmountPreflopInCents { get; set; }

        [ProtoMember(379)]
        public virtual double RaiseSizeToPotPreflop { get; set; }

        [ProtoMember(380)]
        public virtual double FacingRaiseSizeToPot { get; set; }

        [ProtoMember(381)]
        public virtual int NumberOfPlayersOnFlop { get; set; }

        [ProtoMember(382)]
        public virtual int BetAmountFlopInCents { get; set; }

        [ProtoMember(383)]
        public virtual int CallAmountFlopInCents { get; set; }

        [ProtoMember(384)]
        public virtual int FlopPotSizeInCents { get; set; }

        [ProtoMember(385)]
        public virtual double FlopStackPotRatio { get; set; }

        [ProtoMember(386)]
        public virtual double FlopRaiseSizeToPot { get; set; }

        [ProtoMember(387)]
        public virtual double FlopFacingBetSizeToPot { get; set; }

        [ProtoMember(388)]
        public virtual double FlopFacingRaiseSizeToPot { get; set; }

        [ProtoMember(389)]
        public virtual int NumberOfPlayersOnTurn { get; set; }

        [ProtoMember(390)]
        public virtual int BetAmountTurnInCents { get; set; }

        [ProtoMember(391)]
        public virtual int CallAmountTurnInCents { get; set; }

        [ProtoMember(392)]
        public virtual int TurnPotSizeInCents { get; set; }

        [ProtoMember(393)]
        public virtual double TurnStackPotRatio { get; set; }

        [ProtoMember(394)]
        public virtual double TurnRaiseSizeToPot { get; set; }

        [ProtoMember(395)]
        public virtual double TurnFacingBetSizeToPot { get; set; }

        [ProtoMember(396)]
        public virtual double TurnFacingRaiseSizeToPot { get; set; }

        [ProtoMember(397)]
        public virtual int NumberOfPlayersOnRiver { get; set; }

        [ProtoMember(398)]
        public virtual int BetAmountRiverInCents { get; set; }

        [ProtoMember(399)]
        public virtual int CallAmountRiverInCents { get; set; }

        [ProtoMember(400)]
        public virtual int RiverPotSizeInCents { get; set; }

        [ProtoMember(401)]
        public virtual double RiverStackPotRatio { get; set; }

        [ProtoMember(402)]
        public virtual double RiverRaiseSizeToPot { get; set; }

        [ProtoMember(403)]
        public virtual double RiverFacingBetSizeToPot { get; set; }

        [ProtoMember(404)]
        public virtual double RiverFacingRaiseSizeToPot { get; set; }

        [ProtoMember(405)]
        public virtual int NumberOfPlayersSawShowdown { get; set; }

        #region Pot/Amount/Stack based 

        public double BetAmountPreflopInBB => BetAmountPreflopInCents / (double)BigBlind * 100;

        public double CallAmountPreflopInBB => CallAmountPreflopInCents / (double)BigBlind * 100;

        public double PostAmountPreflopInBB => PostAmountPreflopInCents / (double)BigBlind * 100;



        #endregion


        #region Workarounds for broken stats

        public virtual int FoldedtothreebetpreflopVirtual
        {
            get
            {
                return Totalhands == 1 && Pfrhands == 1 && Foldedtothreebetpreflop == 1 ? 1 : 0;
            }
        }

        public virtual int FacedthreebetpreflopVirtual
        {
            get
            {
                return Totalhands == 1 && Position != EnumPosition.BB && Pfrhands == 1 && Facedthreebetpreflop == 1 ? 1 : 0;
            }
        }

        // 4-bet workaround
        public virtual int DidfourbetpreflopVirtual
        {
            get
            {
                return Totalhands == 1 && FacedthreebetpreflopVirtual == 1 && Didfourbet == 1 ? 1 : 0;
            }
        }

        public virtual int CouldfourbetpreflopVirtual
        {
            get
            {
                return Totalhands == 1 && FacedthreebetpreflopVirtual == 1 && Couldfourbet == 1 ? 1 : 0;
            }
        }

        #endregion

        #region Simple IP/OOP based stats (not for serialization)

        public virtual int DidDelayedTurnCBetIP
        {
            get
            {
                return PreflopIP == 1 && DidDelayedTurnCBet == 1 ? 1 : 0;
            }
        }

        public virtual int CouldDelayedTurnCBetIP
        {
            get
            {
                return PreflopIP == 1 && CouldDelayedTurnCBet == 1 ? 1 : 0;
            }
        }

        public virtual int DidDelayedTurnCBetOOP
        {
            get
            {
                return PreflopIP == 0 && DidDelayedTurnCBet == 1 ? 1 : 0;
            }
        }

        public virtual int CouldDelayedTurnCBetOOP
        {
            get
            {
                return PreflopIP == 0 && CouldDelayedTurnCBet == 1 ? 1 : 0;
            }
        }

        #endregion

        #region Stats built from existing properties

        public virtual int CheckRaisedFlopCBet
        {
            get
            {
                return DidCheckFlop == 1 && Raisedflopcontinuationbet == 1 ? 1 : 0;
            }
        }

        public virtual int CouldCheckRaiseFlopCBet
        {
            get
            {
                return DidCheckFlop == 1 && Facingflopcontinuationbet == 1 ? 1 : 0;
            }
        }

        public virtual int DidFlopBet
        {
            get
            {
                return FlopBetToPotRatio > 0 ? 1 : 0;
            }
        }

        public virtual int DidTurnBet
        {
            get
            {
                return TurnBetToPotRatio > 0 ? 1 : 0;
            }
        }

        public virtual int RaisedFlopCBetIn3BetPot
        {
            get
            {
                return Raisedflopcontinuationbet == 1 && In3BetPot ? 1 : 0;
            }
        }

        public virtual int CouldRaiseFlopCBetIn3BetPot
        {
            get
            {
                return Facingflopcontinuationbet == 1 && CouldRaiseFlop == 1 && In3BetPot ? 1 : 0;
            }
        }

        public virtual bool In3BetPot => Didthreebet != 0 || FacedthreebetpreflopVirtual != 0 ||
            (Couldthreebet != 0 && Line.StartsWith("CC", StringComparison.Ordinal) && Facedfourbetpreflop == 0 && Didfourbet == 0);

        public virtual bool In4BetPot => PreflopRaisersCount == 3 && !PreflopActions.EndsWith("F");

        public virtual bool IsSRP => (PreflopActions.Equals("C") || PreflopActions.Equals("F")) &&
            (FacingPreflop == EnumFacingPreflop.Raiser || FacingPreflop == EnumFacingPreflop.RaiserAndCaller) ||
            (FacingPreflop == EnumFacingPreflop.Unopened && PreflopActions.Equals("CC")) ||
            PreflopActions.Equals("R");


        #region FlopBetSize stats

        public virtual int FlopBetSizeOneHalfOrLess
        {
            get
            {
                return FlopBetToPotRatio > 0 && FlopBetToPotRatio <= 0.5m ? 1 : 0;
            }
        }

        public virtual int FlopBetSizeOneQuarterOrLess
        {
            get
            {
                return FlopBetToPotRatio > 0 && FlopBetToPotRatio <= 0.25m ? 1 : 0;
            }
        }

        public virtual int FlopBetSizeTwoThirdsOrLess
        {
            get
            {
                return FlopBetToPotRatio > 0 && FlopBetToPotRatio <= 2 / 3m ? 1 : 0;
            }
        }

        public virtual int FlopBetSizeThreeQuartersOrLess
        {
            get
            {
                return FlopBetToPotRatio > 0 && FlopBetToPotRatio <= 0.75m ? 1 : 0;
            }
        }

        public virtual int FlopBetSizeOneOrLess
        {
            get
            {
                return FlopBetToPotRatio > 0 && FlopBetToPotRatio <= 1m ? 1 : 0;
            }
        }

        public virtual int FlopBetSizeMoreThanOne
        {
            get
            {
                return FlopBetToPotRatio > 1.01m ? 1 : 0;
            }
        }

        public virtual int FoldToTurnCBetIn3BetPot
        {
            get
            {
                return Foldedtoturncontinuationbet == 1 && In3BetPot ? 1 : 0;
            }
        }

        public virtual int FacedToTurnCBetIn3BetPot
        {
            get
            {
                return Facingturncontinuationbet == 1 && In3BetPot ? 1 : 0;
            }
        }

        #endregion

        #region TurnBetSize stats

        public virtual int TurnBetSizeOneHalfOrLess
        {
            get
            {
                return TurnBetToPotRatio > 0 && TurnBetToPotRatio <= 0.5m ? 1 : 0;
            }
        }

        public virtual int TurnBetSizeOneThirdOrLess
        {
            get
            {
                return TurnBetToPotRatio > 0 && TurnBetToPotRatio <= 1 / 3m ? 1 : 0;
            }
        }

        public virtual int TurnBetSizeOneQuarterOrLess
        {
            get
            {
                return TurnBetToPotRatio > 0 && TurnBetToPotRatio <= 0.25m ? 1 : 0;
            }
        }

        public virtual int TurnBetSizeTwoThirdsOrLess
        {
            get
            {
                return TurnBetToPotRatio > 0 && TurnBetToPotRatio <= 2 / 3m ? 1 : 0;
            }
        }

        public virtual int TurnBetSizeThreeQuartersOrLess
        {
            get
            {
                return TurnBetToPotRatio > 0 && TurnBetToPotRatio <= 0.75m ? 1 : 0;
            }
        }

        public virtual int TurnBetSizeOneOrLess
        {
            get
            {
                return TurnBetToPotRatio > 0 && TurnBetToPotRatio <= 1m ? 1 : 0;
            }
        }

        public virtual int TurnBetSizeMoreThanOne
        {
            get
            {
                return TurnBetToPotRatio > 1.01m ? 1 : 0;
            }
        }

        #endregion

        #region RiverBetSize stats

        public virtual int RiverBetSizeMoreThanOne
        {
            get
            {
                return RiverBetToPotRatio > 1.01m ? 1 : 0;
            }
        }

        #endregion

        #region WTSD after stats

        public virtual int WTSDAfterCalling3Bet
        {
            get
            {
                return Calledthreebetpreflop == 1 && Sawshowdown == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAfterCalling3BetOpportunity
        {
            get
            {
                return Calledthreebetpreflop == 1 && Sawflop == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAfterCallingPfr
        {
            get
            {
                return Couldthreebet == 1 && Didthreebet == 0 && Sawshowdown == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAfterCallingPfrOpportunity
        {
            get
            {
                return Couldthreebet == 1 && Didthreebet == 0 && Sawflop == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAfterNotCBettingFlopAsPfr
        {
            get
            {
                return Pfrhands == 1 && Flopcontinuationbetpossible == 1 && Flopcontinuationbetmade == 0 && Sawshowdown == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAfterNotCBettingFlopAsPfrOpportunity
        {
            get
            {
                return Pfrhands == 1 && Flopcontinuationbetpossible == 1 && Flopcontinuationbetmade == 0 ? 1 : 0;
            }
        }

        public virtual int WTSDAfterSeeingTurn
        {
            get
            {
                return SawTurn == 1 && Sawshowdown == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAsPF3Bettor
        {
            get
            {
                return Didthreebet == 1 && Sawshowdown == 1 ? 1 : 0;
            }
        }

        public virtual int WTSDAsPF3BettorOpportunity
        {
            get
            {
                return Didthreebet == 1 && Sawflop == 1 ? 1 : 0;
            }
        }

        #endregion

        #region Raise Limpers stats    

        public virtual int CouldRaiseLimpers => (FacingPreflop == EnumFacingPreflop.Limper || FacingPreflop == EnumFacingPreflop.MultipleLimpers) ? 1 : 0;

        public virtual int RaisedLimpersMP => Position.IsMPPosition() ? IsRaisedLimpers : 0;

        public virtual int RaisedLimpersCO => Position.IsCOPosition() ? IsRaisedLimpers : 0;

        public virtual int RaisedLimpersBN => Position.IsBTNPosition() ? IsRaisedLimpers : 0;

        public virtual int RaisedLimpersSB => Position.IsSBPosition() ? IsRaisedLimpers : 0;

        public virtual int RaisedLimpersBB => Position.IsBBPosition() ? IsRaisedLimpers : 0;

        public virtual int CouldRaiseLimpersMP => Position.IsMPPosition() ? CouldRaiseLimpers : 0;

        public virtual int CouldRaiseLimpersCO => Position.IsCOPosition() ? CouldRaiseLimpers : 0;

        public virtual int CouldRaiseLimpersBN => Position.IsBTNPosition() ? CouldRaiseLimpers : 0;

        public virtual int CouldRaiseLimpersSB => Position.IsSBPosition() ? CouldRaiseLimpers : 0;

        public virtual int CouldRaiseLimpersBB => Position.IsBBPosition() ? CouldRaiseLimpers : 0;

        #endregion

        #region 3-Bet vs Pos stats

        public virtual int ThreeBetMPvsEP => FirstRaiserPosition.IsEPPosition() ? DidThreeBetMP : 0;

        public virtual int CouldThreeBetMPvsEP => FirstRaiserPosition.IsEPPosition() ? CouldThreeBetMP : 0;

        public virtual int ThreeBetCOvsEP => FirstRaiserPosition.IsEPPosition() ? DidThreeBetCO : 0;

        public virtual int CouldThreeBetCOvsEP => FirstRaiserPosition.IsEPPosition() ? CouldThreeBetCO : 0;

        public virtual int ThreeBetCOvsMP => FirstRaiserPosition.IsMPPosition() ? DidThreeBetCO : 0;

        public virtual int CouldThreeBetCOvsMP => FirstRaiserPosition.IsMPPosition() ? CouldThreeBetCO : 0;

        public virtual int ThreeBetBTNvsEP => FirstRaiserPosition.IsEPPosition() ? DidThreeBetBN : 0;

        public virtual int CouldThreeBetBTNvsEP => FirstRaiserPosition.IsEPPosition() ? CouldThreeBetBN : 0;

        public virtual int ThreeBetBTNvsMP => FirstRaiserPosition.IsMPPosition() ? DidThreeBetBN : 0;

        public virtual int CouldThreeBetBTNvsMP => FirstRaiserPosition.IsMPPosition() ? CouldThreeBetBN : 0;

        public virtual int ThreeBetBTNvsCO => FirstRaiserPosition.IsCOPosition() ? DidThreeBetBN : 0;

        public virtual int CouldThreeBetBTNvsCO => FirstRaiserPosition.IsCOPosition() ? CouldThreeBetBN : 0;

        public virtual int ThreeBetSBvsEP => FirstRaiserPosition.IsEPPosition() ? DidThreeBetSB : 0;

        public virtual int CouldThreeBetSBvsEP => FirstRaiserPosition.IsEPPosition() ? CouldThreeBetSB : 0;

        public virtual int ThreeBetSBvsMP => FirstRaiserPosition.IsMPPosition() ? DidThreeBetSB : 0;

        public virtual int CouldThreeBetSBvsMP => FirstRaiserPosition.IsMPPosition() ? CouldThreeBetSB : 0;

        public virtual int ThreeBetSBvsCO => FirstRaiserPosition.IsCOPosition() ? DidThreeBetSB : 0;

        public virtual int CouldThreeBetSBvsCO => FirstRaiserPosition.IsCOPosition() ? CouldThreeBetSB : 0;

        public virtual int ThreeBetSBvsBTN => FirstRaiserPosition.IsBTNPosition() ? DidThreeBetSB : 0;

        public virtual int CouldThreeBetSBvsBTN => FirstRaiserPosition.IsBTNPosition() ? CouldThreeBetSB : 0;

        public virtual int ThreeBetBBvsEP => FirstRaiserPosition.IsEPPosition() ? DidThreeBetBB : 0;

        public virtual int CouldThreeBetBBvsEP => FirstRaiserPosition.IsEPPosition() ? CouldThreeBetBB : 0;

        public virtual int ThreeBetBBvsMP => FirstRaiserPosition.IsMPPosition() ? DidThreeBetBB : 0;

        public virtual int CouldThreeBetBBvsMP => FirstRaiserPosition.IsMPPosition() ? CouldThreeBetBB : 0;

        public virtual int ThreeBetBBvsCO => FirstRaiserPosition.IsCOPosition() ? DidThreeBetBB : 0;

        public virtual int CouldThreeBetBBvsCO => FirstRaiserPosition.IsCOPosition() ? CouldThreeBetBB : 0;

        public virtual int ThreeBetBBvsBTN => FirstRaiserPosition.IsBTNPosition() ? DidThreeBetBB : 0;

        public virtual int CouldThreeBetBBvsBTN => FirstRaiserPosition.IsBTNPosition() ? CouldThreeBetBB : 0;

        public virtual int ThreeBetBBvsSB => FirstRaiserPosition.IsSBPosition() ? DidThreeBetBB : 0;

        public virtual int CouldThreeBetBBvsSB => FirstRaiserPosition.IsSBPosition() ? CouldThreeBetBB : 0;

        #endregion

        #region Fold to 3-Bet in Pos vs 3-bet Pos

        public virtual int FoldTo3BetInEPvs3BetMP => Position.IsEPPosition() && ThreeBettorPosition.IsMPPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInEPvs3BetMP => Position.IsEPPosition() && ThreeBettorPosition.IsMPPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInEPvs3BetCO => Position.IsEPPosition() && ThreeBettorPosition.IsCOPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInEPvs3BetCO => Position.IsEPPosition() && ThreeBettorPosition.IsCOPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInEPvs3BetBTN => Position.IsEPPosition() && ThreeBettorPosition.IsBTNPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInEPvs3BetBTN => Position.IsEPPosition() && ThreeBettorPosition.IsBTNPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInEPvs3BetSB => Position.IsEPPosition() && ThreeBettorPosition.IsSBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInEPvs3BetSB => Position.IsEPPosition() && ThreeBettorPosition.IsSBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInEPvs3BetBB => Position.IsEPPosition() && ThreeBettorPosition.IsBBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInEPvs3BetBB => Position.IsEPPosition() && ThreeBettorPosition.IsBBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInMPvs3BetCO => Position.IsMPPosition() && ThreeBettorPosition.IsCOPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInMPvs3BetCO => Position.IsMPPosition() && ThreeBettorPosition.IsCOPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInMPvs3BetBTN => Position.IsMPPosition() && ThreeBettorPosition.IsBTNPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInMPvs3BetBTN => Position.IsMPPosition() && ThreeBettorPosition.IsBTNPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInMPvs3BetSB => Position.IsMPPosition() && ThreeBettorPosition.IsSBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInMPvs3BetSB => Position.IsMPPosition() && ThreeBettorPosition.IsSBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInMPvs3BetBB => Position.IsMPPosition() && ThreeBettorPosition.IsBBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInMPvs3BetBB => Position.IsMPPosition() && ThreeBettorPosition.IsBBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInCOvs3BetBTN => Position.IsCOPosition() && ThreeBettorPosition.IsBTNPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInCOvs3BetBTN => Position.IsCOPosition() && ThreeBettorPosition.IsBTNPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInCOvs3BetSB => Position.IsCOPosition() && ThreeBettorPosition.IsSBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInCOvs3BetSB => Position.IsCOPosition() && ThreeBettorPosition.IsSBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInCOvs3BetBB => Position.IsCOPosition() && ThreeBettorPosition.IsBBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInCOvs3BetBB => Position.IsCOPosition() && ThreeBettorPosition.IsBBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInBTNvs3BetSB => Position.IsBTNPosition() && ThreeBettorPosition.IsSBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInBTNvs3BetSB => Position.IsBTNPosition() && ThreeBettorPosition.IsSBPosition() ? FacedthreebetpreflopVirtual : 0;

        public virtual int FoldTo3BetInBTNvs3BetBB => Position.IsBTNPosition() && ThreeBettorPosition.IsBBPosition() ? FoldedtothreebetpreflopVirtual : 0;

        public virtual int CouldFoldTo3BetInBTNvs3BetBB => Position.IsBTNPosition() && ThreeBettorPosition.IsBBPosition() ? FacedthreebetpreflopVirtual : 0;

        #endregion

        #region Others

        public virtual int CheckRaiseFlopAsPFR => DidFlopCheckRaise != 0 && Pfrhands != 0 ? 1 : 0;

        public virtual int CouldCheckRaiseFlopAsPFR => CouldFlopCheckRaise != 0 && Pfrhands != 0 ? 1 : 0;

        public virtual int ProbeBetTurn => CouldProbeBetTurn != 0 && DidTurnBet != 0 ? 1 : 0;

        public virtual int ProbeBetRiver => CouldProbeBetRiver != 0 && DidRiverBet != 0 ? 1 : 0;

        public virtual int FloatFlopThenBetTurn => PlayedFloatFlop != 0 && DidTurnBet != 0 ? 1 : 0;

        public virtual int CouldFloatFlopThenBetTurn => PlayedFloatFlop != 0 && CouldTurnBet != 0 ? 1 : 0;

        public virtual int FoldBBvsSBSteal => Bigblindstealfolded != 0 && FirstRaiserPosition.IsSBPosition() ? 1 : 0;

        public virtual int CouldFoldBBvsSBSteal => Bigblindstealfaced != 0 && FirstRaiserPosition.IsSBPosition() ? 1 : 0;

        public virtual int CouldBetTurnWhenCheckedToSRP => CouldBetFlopWhenCheckedToSRP != 0 && DidCheckFlop != 0 && CouldTurnBet != 0 ? 1 : 0;

        public virtual int BetTurnWhenCheckedToSRP => CouldBetFlopWhenCheckedToSRP != 0 && DidCheckFlop != 0 && DidTurnBet != 0 ? 1 : 0;

        public virtual int CouldBetRiverWhenCheckedToSRP => CouldBetTurnWhenCheckedToSRP != 0 && DidCheckTurn != 0 && CouldRiverBet != 0 ? 1 : 0;

        public virtual int BetRiverWhenCheckedToSRP => CouldBetTurnWhenCheckedToSRP != 0 && DidCheckTurn != 0 && DidRiverBet != 0 ? 1 : 0;

        public virtual int DoubleBarrelSRP => Turncontinuationbetmade != 0 && FirstRaiser != 0 &&
            FacedthreebetpreflopVirtual == 0 && Facedfourbetpreflop == 0 && Faced5Bet == 0 ? 1 : 0;

        public virtual int CouldDoubleBarrelSRP => Turncontinuationbetpossible != 0 && FirstRaiser != 0 &&
           FacedthreebetpreflopVirtual == 0 && Facedfourbetpreflop == 0 && Faced5Bet == 0 ? 1 : 0;

        public virtual int DoubleBarrel3BetPot => Turncontinuationbetmade != 0 && In3BetPot ? 1 : 0;

        public virtual int CouldDoubleBarrel3BetPot => Turncontinuationbetpossible != 0 && In3BetPot ? 1 : 0;

        public virtual int TripleBarrelSRP => Rivercontinuationbetmade != 0 && FirstRaiser != 0 &&
            FacedthreebetpreflopVirtual == 0 && Facedfourbetpreflop == 0 && Faced5Bet == 0 ? 1 : 0;

        public virtual int CouldTripleBarrelSRP => Rivercontinuationbetpossible != 0 && FirstRaiser != 0 &&
            FacedthreebetpreflopVirtual == 0 && Facedfourbetpreflop == 0 && Faced5Bet == 0 ? 1 : 0;

        public virtual int TripleBarrel3BetPot => Rivercontinuationbetmade != 0 && In3BetPot ? 1 : 0;

        public virtual int CouldTripleBarrel3BetPot => Rivercontinuationbetpossible != 0 && In3BetPot ? 1 : 0;

        public virtual int CBetThenFoldFlopSRP => Flopcontinuationbetmade != 0 && BetFoldFlopPfrRaiser != 0 && FirstRaiser != 0 &&
            FacedthreebetpreflopVirtual == 0 && Facedfourbetpreflop == 0 && Faced5Bet == 0 ? 1 : 0;

        public virtual int CouldCBetThenFoldFlopSRP => Flopcontinuationbetpossible != 0 && CouldBetFoldFlopPfrRaiser != 0 && FirstRaiser != 0 &&
           FacedthreebetpreflopVirtual == 0 && Facedfourbetpreflop == 0 && Faced5Bet == 0 ? 1 : 0;

        #endregion

        #region Bet When Checked to in 3Bet Pot

        public virtual int BetFlopWhenCheckedToIn3BetPot => DidFlopBet != 0 && PreflopIP != 0 && In3BetPot ? 1 : 0;

        public virtual int CouldBetFlopWhenCheckedToIn3BetPot => CouldFlopBet != 0 && PreflopIP != 0 && In3BetPot ? 1 : 0;

        public virtual int BetTurnWhenCheckedToIn3BetPot => DidTurnBet != 0 && DidCheckFlop != 0 && CouldBetFlopWhenCheckedToIn3BetPot != 0 ? 1 : 0;

        public virtual int CouldBetTurnWhenCheckedToIn3BetPot => CouldTurnBet != 0 && DidCheckFlop != 0 && CouldBetFlopWhenCheckedToIn3BetPot != 0 ? 1 : 0;

        public virtual int BetRiverWhenCheckedToIn3BetPot => DidRiverBet != 0 && DidCheckTurn != 0 && CouldBetTurnWhenCheckedToIn3BetPot != 0 ? 1 : 0;

        public virtual int CouldBetRiverWhenCheckedToIn3BetPot => CouldRiverBet != 0 && DidCheckTurn != 0 && CouldBetTurnWhenCheckedToIn3BetPot != 0 ? 1 : 0;

        #endregion

        #region Fold to probe bet

        public virtual int FacedProbeBetTurn => PreflopIP != 0 && Flopcontinuationbetpossible != 0 && FacedBetOnTurn != 0 && Line.Contains("R-X-") ? 1 : 0;

        public virtual int FoldedToProbeBetTurn => PreflopIP != 0 && Flopcontinuationbetpossible != 0 && FacedBetOnTurn != 0 && Line.Contains("R-X-F") ? 1 : 0;

        public virtual int FacedProbeBetRiver => PreflopIP != 0 && Flopcontinuationbetpossible != 0 && FacedBetOnRiver != 0 && Line.Contains("R-X-X-") ? 1 : 0;

        public virtual int FoldedToProbeBetRiver => PreflopIP != 0 && Flopcontinuationbetpossible != 0 && FacedBetOnRiver != 0 && Line.Contains("R-X-X-F") ? 1 : 0;

        #endregion

        #region Check Flop as PFR and Fold to Turn/River Bet SRP

        public virtual int CheckFlopAsPFRAndFoldToTurnBetIPSRP => PreflopIP != 0 && FirstRaiser != 0 &&
         PreflopActions.Equals("R") && FlopActions.StartsWith("X") && TurnActions.Equals("F") ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP => PreflopIP != 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnTurn != 0 ? 1 : 0;

        public virtual int CheckFlopAsPFRAndFoldToTurnBetOOPSRP => PreflopIP == 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && (TurnActions.Equals("F") || TurnActions.Equals("XF")) ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP => PreflopIP == 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnTurn != 0 ? 1 : 0;

        public virtual int CheckFlopAsPFRAndFoldToRiverBetIPSRP => PreflopIP != 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && RiverActions.Equals("F") ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToRiverBetIPSRP => PreflopIP != 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnRiver != 0 ? 1 : 0;

        public virtual int CheckFlopAsPFRAndFoldToRiverBetOOPSRP => PreflopIP == 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && (RiverActions.Equals("F") || RiverActions.Equals("XF")) ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToRiverBetOOPSRP => PreflopIP == 0 && FirstRaiser != 0 &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnRiver != 0 ? 1 : 0;

        #endregion

        #region Check Flop as PFR and Fold to Turn/River Bet in 3-Bet Pot   

        public virtual int CheckFlopAsPFRAndFoldToTurnBetIP3BetPot => PreflopIP != 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && TurnActions.Equals("F") ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot => PreflopIP != 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnTurn != 0 ? 1 : 0;

        public virtual int CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot => PreflopIP == 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && (TurnActions.Equals("F") || TurnActions.Equals("XF")) ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot => PreflopIP == 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnTurn != 0 ? 1 : 0;

        public virtual int CheckFlopAsPFRAndFoldToRiverBetIP3BetPot => PreflopIP != 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && RiverActions.Equals("F") ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot => PreflopIP != 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnRiver != 0 ? 1 : 0;

        public virtual int CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot => PreflopIP == 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && (RiverActions.Equals("F") || RiverActions.Equals("XF")) ? 1 : 0;

        public virtual int CouldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot => PreflopIP == 0 && In3BetPot &&
            PreflopActions.Equals("R") && FlopActions.StartsWith("X") && FacedBetOnRiver != 0 ? 1 : 0;

        #endregion

        #region Fold to continuation bets in SRP/3Bet/4Bet

        public virtual int FoldToTripleBarrelSRP => IsSRP && Foldedtorivercontinuationbet != 0 ? 1 : 0;

        public virtual int FacingTripleBarrelSRP => IsSRP && Facingrivercontinuationbet != 0 ? 1 : 0;

        public virtual int FoldToTripleBarrel3BetPot => In3BetPot && Foldedtorivercontinuationbet != 0 ? 1 : 0;

        public virtual int FacingTripleBarrel3BetPot => In3BetPot && Facingrivercontinuationbet != 0 ? 1 : 0;

        public virtual int FoldToTripleBarrel4BetPot => In4BetPot && Foldedtorivercontinuationbet != 0 ? 1 : 0;

        public virtual int FacingTripleBarrel4BetPot => In4BetPot && Facingrivercontinuationbet != 0 ? 1 : 0;

        public virtual int FoldToDoubleBarrelSRP => IsSRP && Foldedtoturncontinuationbet != 0 ? 1 : 0;

        public virtual int FacingDoubleBarrelSRP => IsSRP && Facingturncontinuationbet != 0 ? 1 : 0;

        public virtual int FoldToDoubleBarrel4BetPot => In4BetPot && Foldedtoturncontinuationbet != 0 ? 1 : 0;

        public virtual int FacingDoubleBarrel4BetPot => In4BetPot && Facingturncontinuationbet != 0 ? 1 : 0;

        public virtual int FoldToCBetSRP => IsSRP && Foldedtoflopcontinuationbet != 0 ? 1 : 0;

        public virtual int FacingCBetSRP => IsSRP && Facingflopcontinuationbet != 0 ? 1 : 0;

        #endregion

        #region Open Shove UO Pot positional

        public virtual int SBOpenShove1to8bbUOPot => IsUnopened && Line.StartsWith("R") &&
            (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int SBOpenShove9to14bbUOPot => IsUnopened && Line.StartsWith("R") &&
            (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int SBOpenShove15to25bbUOPot => IsUnopened && Line.StartsWith("R") &&
            (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int SBOpenShove26to50bbUOPot => IsUnopened && Line.StartsWith("R") &&
            (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int SBOpenShove51plusbbUOPot => IsUnopened && Line.StartsWith("R") &&
            (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int SBOpenShoveUOPot => IsUnopened && Line.StartsWith("R") && (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() ? 1 : 0;

        public virtual int BTNOpenShove1to8bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int BTNOpenShove9to14bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int BTNOpenShove15to25bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int BTNOpenShove26to50bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int BTNOpenShove51plusbbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int BTNOpenShoveUOPot => IsUnopened && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() ? 1 : 0;

        public virtual int COOpenShove1to8bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int COOpenShove9to14bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int COOpenShove15to25bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int COOpenShove26to50bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int COOpenShove51plusbbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int COOpenShoveUOPot => IsUnopened && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() ? 1 : 0;

        public virtual int MPOpenShove1to8bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int MPOpenShove9to14bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int MPOpenShove15to25bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int MPOpenShove26to50bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int MPOpenShove51plusbbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int MPOpenShoveUOPot => IsUnopened && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() ? 1 : 0;

        public virtual int EPOpenShove1to8bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int EPOpenShove9to14bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int EPOpenShove15to25bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int EPOpenShove26to50bbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int EPOpenShove51plusbbUOPot => IsUnopened && Line.StartsWith("R") &&
            OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int EPOpenShoveUOPot => IsUnopened && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() ? 1 : 0;

        #endregion

        #region Limp Positional & Fold to PFR%

        private bool LimpFoldToPfr => (FacingPreflop == EnumFacingPreflop.Unopened || FacingPreflop == EnumFacingPreflop.Limper ||
            FacingPreflop == EnumFacingPreflop.MultipleLimpers) && PreflopActions.StartsWith("CF");

        private bool LimpFacedPfr => (FacingPreflop == EnumFacingPreflop.Unopened || FacingPreflop == EnumFacingPreflop.Limper ||
         FacingPreflop == EnumFacingPreflop.MultipleLimpers) && PreflopActions.StartsWith("C") && PreflopActions.Length > 1;

        public int LimpEPFoldToPFR => Position.IsEPPosition() && LimpFoldToPfr ? 1 : 0;

        public int LimpEPFacedPFR => Position.IsEPPosition() && LimpFacedPfr ? 1 : 0;

        public int LimpMPFoldToPFR => Position.IsMPPosition() && LimpFoldToPfr ? 1 : 0;

        public int LimpMPFacedPFR => Position.IsMPPosition() && LimpFacedPfr ? 1 : 0;

        public int LimpCOFoldToPFR => Position.IsCOPosition() && LimpFoldToPfr ? 1 : 0;

        public int LimpCOFacedPFR => Position.IsCOPosition() && LimpFacedPfr ? 1 : 0;

        public int LimpBTNFoldToPFR => Position.IsBTNPosition() && LimpFoldToPfr ? 1 : 0;

        public int LimpBTNFacedPFR => Position.IsBTNPosition() && LimpFacedPfr ? 1 : 0;

        public int LimpSBFoldToPFR => Position.IsSBPosition() && LimpFoldToPfr ? 1 : 0;

        public int LimpSBFacedPFR => Position.IsSBPosition() && LimpFacedPfr ? 1 : 0;

        #endregion

        #region Shoves over limpers positional

        private bool FacedLimpers => FacingPreflop == EnumFacingPreflop.Limper || FacingPreflop == EnumFacingPreflop.MultipleLimpers;

        public virtual int SBShoveOverLimpers1to8bb => SBShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int SBShoveOverLimpers9to14bb => SBShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int SBShoveOverLimpers15to25bb => SBShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int SBShoveOverLimpers26to50bb => SBShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int SBShoveOverLimpers51plusbb => SBShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int SBShoveOverLimpers => FacedLimpers && Line.StartsWith("R") &&
            (OpenRaisePreflopInBBs + SmallBlind) >= StackInBBs && Position.IsSBPosition() ? 1 : 0;

        public virtual int BTNShoveOverLimpers1to8bb => BTNShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int BTNShoveOverLimpers9to14bb => BTNShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int BTNShoveOverLimpers15to25bb => BTNShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int BTNShoveOverLimpers26to50bb => BTNShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int BTNShoveOverLimpers51plusbb => BTNShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int BTNShoveOverLimpers => FacedLimpers && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsBTNPosition() ? 1 : 0;

        public virtual int COShoveOverLimpers1to8bb => COShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int COShoveOverLimpers9to14bb => COShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int COShoveOverLimpers15to25bb => COShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int COShoveOverLimpers26to50bb => COShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int COShoveOverLimpers51plusbb => COShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int COShoveOverLimpers => FacedLimpers && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsCOPosition() ? 1 : 0;

        public virtual int MPShoveOverLimpers1to8bb => MPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int MPShoveOverLimpers9to14bb => MPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int MPShoveOverLimpers15to25bb => MPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int MPShoveOverLimpers26to50bb => MPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int MPShoveOverLimpers51plusbb => MPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int MPShoveOverLimpers => FacedLimpers && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsMPPosition() ? 1 : 0;

        public virtual int EPShoveOverLimpers1to8bb => EPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 1 && OpenRaisePreflopInBBs < 9 ? 1 : 0;

        public virtual int EPShoveOverLimpers9to14bb => EPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 9 && OpenRaisePreflopInBBs < 15 ? 1 : 0;

        public virtual int EPShoveOverLimpers15to25bb => EPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 15 && OpenRaisePreflopInBBs < 26 ? 1 : 0;

        public virtual int EPShoveOverLimpers26to50bb => EPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 26 && OpenRaisePreflopInBBs < 51 ? 1 : 0;

        public virtual int EPShoveOverLimpers51plusbb => EPShoveOverLimpers == 1 && OpenRaisePreflopInBBs >= 51 ? 1 : 0;

        public virtual int EPShoveOverLimpers => FacedLimpers && Line.StartsWith("R") && OpenRaisePreflopInBBs >= StackInBBs && Position.IsEPPosition() ? 1 : 0;

        #endregion

        #region Open minraise

        public virtual int OpenMinraise => Pfrhands == 1 && OpenRaisePreflopInBBs == 2 ? 1 : 0;

        public bool IsOpenMinraiseUO => IsUnopened && OpenMinraise == 1;

        public virtual int EPOpenMinraiseUO => IsOpenMinraiseUO && Position.IsEPPosition() ? 1 : 0;

        public virtual int MPOpenMinraiseUO => IsOpenMinraiseUO && Position.IsMPPosition() ? 1 : 0;

        public virtual int COOpenMinraiseUO => IsOpenMinraiseUO && Position.IsCOPosition() ? 1 : 0;

        public virtual int BTNOpenMinraiseUO => IsOpenMinraiseUO && Position.IsBTNPosition() ? 1 : 0;

        public virtual int SBOpenMinraiseUO => IsOpenMinraiseUO && Position.IsSBPosition() ? 1 : 0;

        public virtual int BBOpenMinraiseUO => IsOpenMinraiseUO && Position.IsBBPosition() ? 1 : 0;

        #endregion

        #region Squeeze positional

        public virtual int DidSqueezeEP => Didsqueeze == 1 && Position.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeEP => Couldsqueeze == 1 && Position.IsEPPosition() ? 1 : 0;

        public virtual int DidSqueezeMP => Didsqueeze == 1 && Position.IsMPPosition() ? 1 : 0;

        public virtual int CouldSqueezeMP => Couldsqueeze == 1 && Position.IsMPPosition() ? 1 : 0;

        public virtual int DidSqueezeCO => Didsqueeze == 1 && Position.IsCOPosition() ? 1 : 0;

        public virtual int CouldSqueezeCO => Couldsqueeze == 1 && Position.IsCOPosition() ? 1 : 0;

        public virtual int DidSqueezeBTN => Didsqueeze == 1 && Position.IsBTNPosition() ? 1 : 0;

        public virtual int CouldSqueezeBTN => Couldsqueeze == 1 && Position.IsBTNPosition() ? 1 : 0;

        public virtual int DidSqueezeSB => Didsqueeze == 1 && Position.IsSBPosition() ? 1 : 0;

        public virtual int CouldSqueezeSB => Couldsqueeze == 1 && Position.IsSBPosition() ? 1 : 0;

        public virtual int DidSqueezeBB => Didsqueeze == 1 && Position.IsBBPosition() ? 1 : 0;

        public virtual int CouldSqueezeBB => Couldsqueeze == 1 && Position.IsBBPosition() ? 1 : 0;

        #endregion

        #region Squeeze vs PFR

        public virtual int DidSqueezeBBVsBTNPFR => DidSqueezeBB == 1 && FirstRaiserPosition.IsBTNPosition() ? 1 : 0;

        public virtual int CouldSqueezeBBVsBTNPFR => CouldSqueezeBB == 1 && FirstRaiserPosition.IsBTNPosition() ? 1 : 0;

        public virtual int DidSqueezeBBVsCOPFR => DidSqueezeBB == 1 && FirstRaiserPosition.IsCOPosition() ? 1 : 0;

        public virtual int CouldSqueezeBBVsCOPFR => CouldSqueezeBB == 1 && FirstRaiserPosition.IsCOPosition() ? 1 : 0;

        public virtual int DidSqueezeBBVsMPPFR => DidSqueezeBB == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int CouldSqueezeBBVsMPPFR => CouldSqueezeBB == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int DidSqueezeBBVsEPPFR => DidSqueezeBB == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeBBVsEPPFR => CouldSqueezeBB == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int DidSqueezeSBVsCOPFR => DidSqueezeSB == 1 && FirstRaiserPosition.IsCOPosition() ? 1 : 0;

        public virtual int CouldSqueezeSBVsCOPFR => CouldSqueezeSB == 1 && FirstRaiserPosition.IsCOPosition() ? 1 : 0;

        public virtual int DidSqueezeSBVsMPPFR => DidSqueezeSB == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int CouldSqueezeSBVsMPPFR => CouldSqueezeSB == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int DidSqueezeSBVsEPPFR => DidSqueezeSB == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeSBVsEPPFR => CouldSqueezeSB == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int DidSqueezeBTNVsMPPFR => DidSqueezeBTN == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int CouldSqueezeBTNVsMPPFR => CouldSqueezeBTN == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int DidSqueezeBTNVsEPPFR => DidSqueezeBTN == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeBTNVsEPPFR => CouldSqueezeBTN == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int DidSqueezeCOVsMPPFR => DidSqueezeCO == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int CouldSqueezeCOVsMPPFR => CouldSqueezeCO == 1 && FirstRaiserPosition.IsMPPosition() ? 1 : 0;

        public virtual int DidSqueezeCOVsEPPFR => DidSqueezeCO == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeCOVsEPPFR => CouldSqueezeCO == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int DidSqueezeMPVsEPPFR => DidSqueezeMP == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeMPVsEPPFR => CouldSqueezeMP == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int DidSqueezeEPVsEPPFR => DidSqueezeEP == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        public virtual int CouldSqueezeEPVsEPPFR => CouldSqueezeEP == 1 && FirstRaiserPosition.IsEPPosition() ? 1 : 0;

        #endregion

        #region Fold to Squeeze as Cold Caller

        public virtual int FoldToSqueezeAsColdCaller => Didcoldcall == 1 && FoldedFacedSqueez == 1 ? 1 : 0;

        public virtual int FacedSqueezeAsColdCaller => Didcoldcall == 1 && FacedSqueez == 1 ? 1 : 0;

        #endregion

        #region 4-Bet vs Blind 3-Bet%

        public virtual int Did4BetVsBlind3Bet => Didfourbet == 1 &&
            (ThreeBettorPosition.IsBBPosition() || ThreeBettorPosition.IsSBPosition()) ? 1 : 0;

        public virtual int Could4BetVsBlind3Bet => Couldfourbet == 1 &&
          (ThreeBettorPosition.IsBBPosition() || ThreeBettorPosition.IsSBPosition()) ? 1 : 0;

        #endregion

        #region BTN Re/Def vs CO Steal

        public virtual int BTNReStealVsCOSteal => FirstRaiserPosition.IsCOPosition() && FacingPreflop == EnumFacingPreflop.Raiser && Pfrhands == 1 ? 1 : 0;

        public virtual int BTNDefendVsCOSteal => FirstRaiserPosition.IsCOPosition() && FacingPreflop == EnumFacingPreflop.Raiser && Vpiphands == 1 ? 1 : 0;

        public virtual int BTNFacedCOSteal => FirstRaiserPosition.IsCOPosition() && FacingPreflop == EnumFacingPreflop.Raiser ? 1 : 0;

        #endregion

        #region Positional Call & Fold to Steal

        public virtual int FacedStealInSB => Position.IsSBPosition() && FacingPreflop == EnumFacingPreflop.Raiser &&
            (FirstRaiserPosition.IsCOPosition() || FirstRaiserPosition.IsBTNPosition()) ? 1 : 0;

        public virtual int FacedStealInBB => Position.IsBBPosition() && FacingPreflop == EnumFacingPreflop.Raiser &&
            (FirstRaiserPosition.IsCOPosition() || FirstRaiserPosition.IsBTNPosition() || FirstRaiserPosition.IsSBPosition()) ? 1 : 0;

        public virtual int FacedCOStealInSB => Position.IsSBPosition() && FacingPreflop == EnumFacingPreflop.Raiser &&
            FirstRaiserPosition.IsCOPosition() ? 1 : 0;

        public virtual int FacedBTNStealInSB => Position.IsSBPosition() && FacingPreflop == EnumFacingPreflop.Raiser &&
            FirstRaiserPosition.IsBTNPosition() ? 1 : 0;

        public virtual int FacedCOStealInBB => Position.IsBBPosition() && FacingPreflop == EnumFacingPreflop.Raiser &&
         FirstRaiserPosition.IsCOPosition() ? 1 : 0;

        public virtual int FacedBTNStealInBB => Position.IsBBPosition() && FacingPreflop == EnumFacingPreflop.Raiser &&
            FirstRaiserPosition.IsBTNPosition() ? 1 : 0;

        public virtual int CalledStealInSB => FacedStealInSB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CalledCOStealInSB => FacedCOStealInSB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CalledBTNStealInSB => FacedBTNStealInSB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CalledStealInBB => FacedStealInBB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CalledCOStealInBB => FacedCOStealInBB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CalledBTNStealInBB => FacedBTNStealInBB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int FoldToStealInSB => FacedStealInSB == 1 && PreflopActions.StartsWith("F") ? 1 : 0;

        public virtual int FoldToCOStealInSB => FacedCOStealInSB == 1 && PreflopActions.StartsWith("F") ? 1 : 0;

        public virtual int FoldToBTNStealInSB => FacedBTNStealInSB == 1 && PreflopActions.StartsWith("F") ? 1 : 0;

        public virtual int FoldToStealInBB => FacedStealInBB == 1 && PreflopActions.StartsWith("F") ? 1 : 0;

        public virtual int FoldToCOStealInBB => FacedCOStealInBB == 1 && PreflopActions.StartsWith("F") ? 1 : 0;

        public virtual int FoldToBTNStealInBB => FacedBTNStealInBB == 1 && PreflopActions.StartsWith("F") ? 1 : 0;

        public virtual int CouldOvercallBTNStealInBB => FacingPreflop == EnumFacingPreflop.RaiserAndCaller && FirstRaiserPosition.IsBTNPosition() ? 1 : 0;

        public virtual int OvercallBTNStealInBB => CouldOvercallBTNStealInBB == 1 && PreflopActions.StartsWith("C") ? 1 : 0;

        #endregion

        #region WTSD as PFR/4-Bettor

        public virtual int WTSDAsPFR => Pfrhands == 1 && Sawshowdown == 1 ? 1 : 0;

        public virtual int WTSDAsPFROpportunity => Pfrhands == 1 && Sawflop == 1 ? 1 : 0;

        public virtual int WTSDAs4Bettor => Didfourbet == 1 && Sawshowdown == 1 ? 1 : 0;

        public virtual int WTSDAs4BettorOpportunity => Didfourbet == 1 && Sawflop == 1 ? 1 : 0;

        #endregion

        #region Call 4-Bet positional 

        public virtual int Call4BetIP => Calledfourbetpreflop == 1 && PreflopIP == 1 ? 1 : 0;

        public virtual int Faced4BetIP => Facedfourbetpreflop == 1 && PreflopIP == 1 ? 1 : 0;

        public virtual int Call4BetOOP => Calledfourbetpreflop == 1 && PreflopIP == 0 ? 1 : 0;

        public virtual int Faced4BetOOP => Facedfourbetpreflop == 1 && PreflopIP == 0 ? 1 : 0;

        public virtual int Call4BetEP => Calledfourbetpreflop == 1 && Position.IsEPPosition() ? 1 : 0;

        public virtual int Faced4BetEP => Facedfourbetpreflop == 1 && Position.IsEPPosition() ? 1 : 0;

        public virtual int Call4BetMP => Calledfourbetpreflop == 1 && Position.IsMPPosition() ? 1 : 0;

        public virtual int Faced4BetMP => Facedfourbetpreflop == 1 && Position.IsMPPosition() ? 1 : 0;

        public virtual int Call4BetCO => Calledfourbetpreflop == 1 && Position.IsCOPosition() ? 1 : 0;

        public virtual int Faced4BetCO => Facedfourbetpreflop == 1 && Position.IsCOPosition() ? 1 : 0;

        public virtual int Call4BetBTN => Calledfourbetpreflop == 1 && Position.IsBTNPosition() ? 1 : 0;

        public virtual int Faced4BetBTN => Facedfourbetpreflop == 1 && Position.IsBTNPosition() ? 1 : 0;

        public virtual int Call4BetSB => Calledfourbetpreflop == 1 && Position.IsSBPosition() ? 1 : 0;

        public virtual int Faced4BetSB => Facedfourbetpreflop == 1 && Position.IsSBPosition() ? 1 : 0;

        public virtual int Call4BetBB => Calledfourbetpreflop == 1 && Position.IsBBPosition() ? 1 : 0;

        public virtual int Faced4BetBB => Facedfourbetpreflop == 1 && Position.IsBBPosition() ? 1 : 0;

        #endregion

        #region Total overcall SRP%

        public virtual int TotalOverCallSRP => (FacingPreflop == EnumFacingPreflop.RaiserAndCaller || FacingPreflop == EnumFacingPreflop.MultipleCallers) &&
            PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CouldTotalOverCallSRP => (FacingPreflop == EnumFacingPreflop.RaiserAndCaller || FacingPreflop == EnumFacingPreflop.MultipleCallers) ? 1 : 0;

        #endregion

        #region Limped pot Flop Steal IP%

        public virtual int LimpedPotFlopStealIP => PreflopIP == 1 && (FacingPreflop == EnumFacingPreflop.Limper || FacingPreflop == EnumFacingPreflop.MultipleLimpers) &&
            PreflopActions.Equals("C") && FlopActions.StartsWith("B") ? 1 : 0;

        public virtual int CouldLimpedPotFlopStealIP => PreflopIP == 1 && (FacingPreflop == EnumFacingPreflop.Limper || FacingPreflop == EnumFacingPreflop.MultipleLimpers) &&
           PreflopActions.Equals("C") && CouldFlopBet == 1 ? 1 : 0;

        #endregion

        #region Flop-Check Call

        public virtual int DidFlopCheckCall => FlopActions.StartsWith("XC") ? 1 : 0;

        public virtual int CouldFlopCheckCall => FlopActions.StartsWith("X") && FlopActions.Length > 1 ? 1 : 0;

        #endregion

        #region Call Flop & Fold Turn

        public virtual int DidCallFlopFoldTurn => FlopActions.EndsWith("C") && TurnActions.EndsWith("F") ? 1 : 0;

        public virtual int CouldCallFlopFoldTurn => FlopActions.EndsWith("C") && FacedBetOnTurn == 1 ? 1 : 0;

        #endregion

        #region River fold in SRP/3-Bet/4-Bet

        public virtual int DidRiverFoldInSRP => IsSRP && RiverActions.EndsWith("F") ? 1 : 0;

        public virtual int CouldRiverFoldInSRP => IsSRP && RiverActions.Length > 0 ? 1 : 0;

        public virtual int DidRiverFoldIn3Bet => In3BetPot && RiverActions.EndsWith("F") ? 1 : 0;

        public virtual int CouldRiverFoldIn3Bet => In3BetPot && RiverActions.Length > 0 ? 1 : 0;

        public virtual int DidRiverFoldIn4Bet => In4BetPot && RiverActions.EndsWith("F") ? 1 : 0;

        public virtual int CouldRiverFoldIn4Bet => In4BetPot && RiverActions.Length > 0 ? 1 : 0;

        #endregion

        #region Delayed Turn C-Bet in SRP/4-Bet Pot%

        public virtual int DidDelayedTurnCBetInSRP => DidDelayedTurnCBet == 1 && IsSRP ? 1 : 0;

        public virtual int CouldDelayedTurnCBetInSRP => CouldDelayedTurnCBet == 1 && IsSRP ? 1 : 0;

        public virtual int DidDelayedTurnCBetIn4BetPot => DidDelayedTurnCBet == 1 && In4BetPot ? 1 : 0;

        public virtual int CouldDelayedTurnCBetIn4BetPot => CouldDelayedTurnCBet == 1 && In4BetPot ? 1 : 0;

        #endregion

        #region Skip Flop C-Bet SRP & C/F Flop OOP%

        public virtual int DidSkipFlopCBetInSRPandCheckFoldFlopOOP => Flopcontinuationbetpossible == 1 && IsSRP && FlopActions.Equals("CF") && PreflopIP == 0 ? 1 : 0;

        public virtual int CouldSkipFlopCBetInSRPandCheckFoldFlopOOP => Flopcontinuationbetpossible == 1 && IsSRP && FlopActions.StartsWith("C") && FlopActions.Length > 1 && PreflopIP == 0 ? 1 : 0;

        #endregion

        #region Check-Raise Flop as PFR SRP/3-Bet pot

        public virtual int DidCheckRaiseFlopAsPFRInSRP => DidFlopCheckRaise == 1 && Pfrhands == 1 && IsSRP ? 1 : 0;

        public virtual int CouldCheckRaiseFlopAsPFRInSRP => CouldFlopCheckRaise == 1 && Pfrhands == 1 && IsSRP ? 1 : 0;

        public virtual int DidCheckRaiseFlopAsPFRIn3BetPot => DidFlopCheckRaise == 1 && Pfrhands == 1 && In3BetPot ? 1 : 0;

        public virtual int CouldCheckRaiseFlopAsPFRIn3BetPot => CouldFlopCheckRaise == 1 && Pfrhands == 1 && In3BetPot ? 1 : 0;

        #endregion

        #region Open Limp

        public virtual int DidOpenLimp => FacingPreflop == EnumFacingPreflop.Unopened && PreflopActions.StartsWith("C") ? 1 : 0;

        public virtual int CouldOpenLimp => FacingPreflop == EnumFacingPreflop.Unopened && PreflopActions.Length > 0 ? 1 : 0;

        public virtual int DidOpenLimpEP => DidOpenLimp == 1 && Position.IsEPPosition() ? 1 : 0;

        public virtual int CouldOpenLimpEP => CouldOpenLimp == 1 && Position.IsEPPosition() ? 1 : 0;

        public virtual int DidOpenLimpMP => DidOpenLimp == 1 && Position.IsMPPosition() ? 1 : 0;

        public virtual int CouldOpenLimpMP => CouldOpenLimp == 1 && Position.IsMPPosition() ? 1 : 0;

        public virtual int DidOpenLimpCO => DidOpenLimp == 1 && Position.IsCOPosition() ? 1 : 0;

        public virtual int CouldOpenLimpCO => CouldOpenLimp == 1 && Position.IsCOPosition() ? 1 : 0;

        public virtual int DidOpenLimpBTN => DidOpenLimp == 1 && Position.IsBTNPosition() ? 1 : 0;

        public virtual int CouldOpenLimpBTN => CouldOpenLimp == 1 && Position.IsBTNPosition() ? 1 : 0;

        public virtual int DidOpenLimpSB => DidOpenLimp == 1 && Position.IsSBPosition() ? 1 : 0;

        public virtual int CouldOpenLimpSB => CouldOpenLimp == 1 && Position.IsSBPosition() ? 1 : 0;

        #endregion

        #region Straddle stats

        public virtual int CouldActInStraddle => IsStraddle && Position.IsStraddlePosition() && PreflopActions.Length > 0 ? 1 : 0;

        public virtual int DidCheckInStraddle => IsStraddle && Position.IsStraddlePosition() && PreflopActions.Equals("C") ? 1 : 0;

        public virtual int DidPFRInStraddle => IsStraddle && Position.IsStraddlePosition() && Pfrhands == 1 ? 1 : 0;

        public virtual int Did3BetInStraddle => IsStraddle && Position.IsStraddlePosition() && Didthreebet == 1 ? 1 : 0;

        public virtual int Could3BetInStraddle => IsStraddle && Position.IsStraddlePosition() && Couldthreebet == 1 ? 1 : 0;

        public virtual int Did4BetInStraddle => IsStraddle && Position.IsStraddlePosition() && Didfourbet == 1 ? 1 : 0;

        public virtual int Could4BetInStraddle => IsStraddle && Position.IsStraddlePosition() && Couldfourbet == 1 ? 1 : 0;

        public virtual int DidFoldInStraddle => IsStraddle && Position.IsStraddlePosition() && PreflopActions.Equals("F") ? 1 : 0;

        public virtual int WTSDInStraddle => IsStraddle && Position.IsStraddlePosition() && Sawshowdown == 1 ? 1 : 0;

        public virtual int WTSDOpportunityInStraddle => IsStraddle && Position.IsStraddlePosition() && Sawflop == 1 ? 1 : 0;

        #endregion

        #region Fold to delayed Turn C-Bet

        public virtual int FoldedToDelayedCBet => FacedDelayedCBet == 1 && (TurnActions.Equals("XF") || TurnActions.Equals("F")) ? 1 : 0;

        #endregion

        #region Check Fold Flop

        public virtual int CouldCheckFoldFlopPfrOop => Pfrhands == 1 && PreflopIP == 0 && FlopActions.StartsWith("X") && FlopActions.Length > 1 ? 1 : 0;

        public virtual int CouldCheckFoldFlop3BetOop => Didthreebet == 1 && PreflopIP == 0 && FlopActions.StartsWith("X") && FlopActions.Length > 1 ? 1 : 0;

        #endregion

        #endregion

        #region Additional properties (not for serialization)

        public virtual bool IsUnopened
        {
            get
            {
                return FacingPreflop == EnumFacingPreflop.Unopened;
            }
        }

        #region Positional stats helpers           

        #region Positional Cold call 

        public virtual int DidColdCallBB => Position.IsBBPosition() ? Didcoldcall : 0;

        public virtual int DidColdCallSB => Position.IsSBPosition() ? Didcoldcall : 0;

        public virtual int DidColdCallEP => Position.IsEPPosition() ? Didcoldcall : 0;

        public virtual int DidColdCallMP => Position.IsMPPosition() ? Didcoldcall : 0;

        public virtual int DidColdCallCO => Position.IsCOPosition() ? Didcoldcall : 0;

        public virtual int DidColdCallBN => Position.IsBTNPosition() ? Didcoldcall : 0;

        public virtual int CouldColdCallBB => Position.IsBBPosition() ? Couldcoldcall : 0;

        public virtual int CouldColdCallSB => Position.IsSBPosition() ? Couldcoldcall : 0;

        public virtual int CouldColdCallEP => Position.IsEPPosition() ? Couldcoldcall : 0;

        public virtual int CouldColdCallMP => Position.IsMPPosition() ? Couldcoldcall : 0;

        public virtual int CouldColdCallCO => Position.IsCOPosition() ? Couldcoldcall : 0;

        public virtual int CouldColdCallBN => Position.IsBTNPosition() ? Couldcoldcall : 0;

        #endregion

        #region Positional Cold call 3-Bet 

        public virtual int DidColdCall3BetInBB => Position.IsBBPosition() ? DidColdCallThreeBet : 0;

        public virtual int DidColdCall3BetInSB => Position.IsSBPosition() ? DidColdCallThreeBet : 0;

        public virtual int DidColdCall3BetInMP => Position.IsMPPosition() ? DidColdCallThreeBet : 0;

        public virtual int DidColdCall3BetInCO => Position.IsCOPosition() ? DidColdCallThreeBet : 0;

        public virtual int DidColdCall3BetInBTN => Position.IsBTNPosition() ? DidColdCallThreeBet : 0;

        public virtual int CouldColdCall3BetInBB => Position.IsBBPosition() ? CouldColdCallThreeBet : 0;

        public virtual int CouldColdCall3BetInSB => Position.IsSBPosition() ? CouldColdCallThreeBet : 0;

        public virtual int CouldColdCall3BetInMP => Position.IsMPPosition() ? CouldColdCallThreeBet : 0;

        public virtual int CouldColdCall3BetInCO => Position.IsCOPosition() ? CouldColdCallThreeBet : 0;

        public virtual int CouldColdCall3BetInBTN => Position.IsBTNPosition() ? CouldColdCallThreeBet : 0;

        #endregion

        #region Positional Cold call 4-Bet 

        public virtual int DidColdCall4BetInBB => Position.IsBBPosition() ? DidColdCallFourBet : 0;

        public virtual int DidColdCall4BetInSB => Position.IsSBPosition() ? DidColdCallFourBet : 0;

        public virtual int DidColdCall4BetInMP => Position.IsMPPosition() ? DidColdCallFourBet : 0;

        public virtual int DidColdCall4BetInCO => Position.IsCOPosition() ? DidColdCallFourBet : 0;

        public virtual int DidColdCall4BetInBTN => Position.IsBTNPosition() ? DidColdCallFourBet : 0;

        public virtual int CouldColdCall4BetInBB => Position.IsBBPosition() ? CouldColdCallFourBet : 0;

        public virtual int CouldColdCall4BetInSB => Position.IsSBPosition() ? CouldColdCallFourBet : 0;

        public virtual int CouldColdCall4BetInMP => Position.IsMPPosition() ? CouldColdCallFourBet : 0;

        public virtual int CouldColdCall4BetInCO => Position.IsCOPosition() ? CouldColdCallFourBet : 0;

        public virtual int CouldColdCall4BetInBTN => Position.IsBTNPosition() ? CouldColdCallFourBet : 0;

        #endregion

        #region Positional VPIP

        public virtual int VPIPBB => Position.IsBBPosition() ? Vpiphands : 0;

        public virtual int VPIPSB => Position.IsSBPosition() ? Vpiphands : 0;

        public virtual int VPIPEP => Position.IsEPPosition() ? Vpiphands : 0;

        public virtual int VPIPMP => Position.IsMPPosition() ? Vpiphands : 0;

        public virtual int VPIPCO => Position.IsCOPosition() ? Vpiphands : 0;

        public virtual int VPIPBN => Position.IsBTNPosition() ? Vpiphands : 0;

        #endregion

        #region Positional 3-bet

        public virtual int DidThreeBetBB => Position.IsBBPosition() ? Didthreebet : 0;

        public virtual int DidThreeBetSB => Position.IsSBPosition() ? Didthreebet : 0;

        public virtual int DidThreeBetEP => Position.IsEPPosition() ? Didthreebet : 0;

        public virtual int DidThreeBetMP => Position.IsMPPosition() ? Didthreebet : 0;

        public virtual int DidThreeBetCO => Position.IsCOPosition() ? Didthreebet : 0;

        public virtual int DidThreeBetBN => Position.IsBTNPosition() ? Didthreebet : 0;

        public virtual int CouldThreeBetBB => Position.IsBBPosition() ? Couldthreebet : 0;

        public virtual int CouldThreeBetSB => Position.IsSBPosition() ? Couldthreebet : 0;

        public virtual int CouldThreeBetEP => Position.IsEPPosition() ? Couldthreebet : 0;

        public virtual int CouldThreeBetMP => Position.IsMPPosition() ? Couldthreebet : 0;

        public virtual int CouldThreeBetCO => Position.IsCOPosition() ? Couldthreebet : 0;

        public virtual int CouldThreeBetBN => Position.IsBTNPosition() ? Couldthreebet : 0;

        #endregion

        #region Positional 4-bet

        public virtual int DidFourBetBB => Position.IsBBPosition() ? Didfourbet : 0;

        public virtual int DidFourBetSB => Position.IsSBPosition() ? Didfourbet : 0;

        public virtual int DidFourBetEP => Position.IsEPPosition() ? Didfourbet : 0;

        public virtual int DidFourBetMP => Position.IsMPPosition() ? Didfourbet : 0;

        public virtual int DidFourBetCO => Position.IsCOPosition() ? Didfourbet : 0;

        public virtual int DidFourBetBN => Position.IsBTNPosition() ? Didfourbet : 0;

        public virtual int CouldFourBetBB => Position.IsBBPosition() ? Couldfourbet : 0;

        public virtual int CouldFourBetSB => Position.IsSBPosition() ? Couldfourbet : 0;

        public virtual int CouldFourBetEP => Position.IsEPPosition() ? Couldfourbet : 0;

        public virtual int CouldFourBetMP => Position.IsMPPosition() ? Couldfourbet : 0;

        public virtual int CouldFourBetCO => Position.IsCOPosition() ? Couldfourbet : 0;

        public virtual int CouldFourBetBN => Position.IsBTNPosition() ? Couldfourbet : 0;

        #endregion

        #region Positional Limp

        public virtual int LimpPossibleSB => Position.IsSBPosition() ? LimpPossible : 0;

        public virtual int LimpPossibleEP => Position.IsEPPosition() ? LimpPossible : 0;

        public virtual int LimpPossibleMP => Position.IsMPPosition() ? LimpPossible : 0;

        public virtual int LimpPossibleCO => Position.IsCOPosition() ? LimpPossible : 0;

        public virtual int LimpPossibleBN => Position.IsBTNPosition() ? LimpPossible : 0;

        #endregion

        #endregion

        public virtual decimal TotalPot { get; set; }

        public virtual decimal TotalPotInBB { get; set; }

        public virtual decimal NetWon
        {
            get { return Totalamountwonincents / 100.0m; }
        }

        public virtual int TiltMeterPermanent
        {
            get; set;
        }

        private Queue<int> tiltMeterTemporaryHistory = new Queue<int>();

        public virtual Queue<int> TiltMeterTemporaryHistory
        {
            get
            {
                return tiltMeterTemporaryHistory;
            }
            set
            {
                tiltMeterTemporaryHistory = value;
            }
        }

        public virtual int DidThreeBetInRow
        {
            get; set;
        }

        public virtual int PFRInRow
        {
            get; set;
        }

        public virtual bool PlayerFolded
        {
            get; set;
        }

        public virtual Handnotes HandNote { get; set; }

        public virtual string HandNoteText
        {
            get
            {
                return HandNote == null ? string.Empty : HandNote.Note;
            }
        }

        public virtual EnumHandTag HandTag
        {
            get
            {
                if (HandNote == null || HandNote.HandTag == null)
                {
                    return EnumHandTag.None;
                }

                return (EnumHandTag)HandNote.HandTag;
            }
        }

        #endregion

        public virtual void Add(Playerstatistic a)
        {
            Sawshowdown += a.Sawshowdown;
            Sawflop += a.Sawflop;
            SawTurn += a.SawTurn;
            SawRiver += a.SawRiver;
            WasFlop += a.WasFlop;
            WasTurn += a.WasTurn;
            WasRiver += a.WasRiver;
            GameNumber = a.GameNumber;
            PokersiteId = a.PokersiteId;
            Pot = a.Pot;
            TotalPot += a.TotalPot;
            TotalPotInBB += a.TotalPotInBB;
            SmallBlind = a.SmallBlind;
            BigBlind = a.BigBlind;
            StartingStack = a.StartingStack;
            Ante = a.Ante;
            CurrencyId = a.CurrencyId;
            Position = a.Position;
            PositionString = a.PositionString;
            PokergametypeId = a.PokergametypeId;

            Wonshowdown += a.Wonshowdown;
            Wonhandwhensawflop += a.Wonhandwhensawflop;
            Wonhandwhensawturn += a.Wonhandwhensawturn;
            Wonhandwhensawriver += a.Wonhandwhensawriver;
            Wonhand += a.Wonhand;

            Vpiphands += a.Vpiphands;
            Pfrhands += a.Pfrhands;

            Couldthreebet += a.Couldthreebet;
            Didthreebet += a.Didthreebet;
            DidThreeBetIp += a.DidThreeBetIp;
            CouldThreeBetIp += a.CouldThreeBetIp;
            DidThreeBetOop += a.DidThreeBetOop;
            CouldThreeBetOop += a.CouldThreeBetOop;
            Facedthreebetpreflop += a.Facedthreebetpreflop;
            Foldedtothreebetpreflop += a.Foldedtothreebetpreflop;
            Calledthreebetpreflop += a.Calledthreebetpreflop;
            Raisedthreebetpreflop += a.Raisedthreebetpreflop;
            Did5Bet += a.Did5Bet;
            Could5Bet += a.Could5Bet;
            Faced5Bet += a.Faced5Bet;
            FoldedTo5Bet += a.FoldedTo5Bet;

            CalledCheckRaiseVsFlopCBet += a.CalledCheckRaiseVsFlopCBet;
            FoldedCheckRaiseVsFlopCBet += a.FoldedCheckRaiseVsFlopCBet;
            FacedCheckRaiseVsFlopCBet += a.FacedCheckRaiseVsFlopCBet;

            Totalbbswon += a.Totalbbswon;
            Totalhands += a.Totalhands;
            Totalbets += a.Totalbets;
            Totalcalls += a.Totalcalls;
            Totalpostflopstreetsplayed += a.Totalpostflopstreetsplayed;
            Totalamountwonincents += a.Totalamountwonincents;
            Totalrakeincents += a.Totalrakeincents;
            Totalaggressivepostflopstreetsseen += a.Totalaggressivepostflopstreetsseen;
            NumberOfWalks += a.NumberOfWalks;

            Flopcontinuationbetpossible += a.Flopcontinuationbetpossible;
            Flopcontinuationbetmade += a.Flopcontinuationbetmade;
            Flopcontinuationipbetmade += a.Flopcontinuationipbetmade;
            Flopcontinuationipbetpossible += a.Flopcontinuationipbetpossible;
            Flopcontinuationoopbetmade += a.Flopcontinuationoopbetmade;
            Flopcontinuationoopbetpossible += a.Flopcontinuationoopbetpossible;
            FlopContinuationBetInThreeBetPotPossible += a.FlopContinuationBetInThreeBetPotPossible;
            FlopContinuationBetInThreeBetPotMade += a.FlopContinuationBetInThreeBetPotMade;
            FlopContinuationBetInFourBetPotPossible += a.FlopContinuationBetInFourBetPotPossible;
            FlopContinuationBetInFourBetPotMade += a.FlopContinuationBetInFourBetPotMade;
            FlopContinuationBetVsOneOpponentPossible += a.FlopContinuationBetVsOneOpponentPossible;
            FlopContinuationBetVsOneOpponentMade += a.FlopContinuationBetVsOneOpponentMade;
            FlopContinuationBetVsTwoOpponentsPossible += a.FlopContinuationBetVsTwoOpponentsPossible;
            FlopContinuationBetVsTwoOpponentsMade += a.FlopContinuationBetVsTwoOpponentsMade;
            MultiWayFlopContinuationBetPossible += a.MultiWayFlopContinuationBetPossible;
            MultiWayFlopContinuationBetMade += a.MultiWayFlopContinuationBetMade;
            FlopContinuationBetMonotonePotPossible += a.FlopContinuationBetMonotonePotPossible;
            FlopContinuationBetMonotonePotMade += a.FlopContinuationBetMonotonePotMade;
            FlopContinuationBetRagPotPossible += a.FlopContinuationBetRagPotPossible;
            FlopContinuationBetRagPotMade += a.FlopContinuationBetRagPotMade;

            Turncontinuationbetpossible += a.Turncontinuationbetpossible;
            Turncontinuationbetmade += a.Turncontinuationbetmade;
            Rivercontinuationbetpossible += a.Rivercontinuationbetpossible;
            Rivercontinuationbetmade += a.Rivercontinuationbetmade;

            TurnContinuationBetWithAirMade += a.TurnContinuationBetWithAirMade;

            Facingflopcontinuationbet += a.Facingflopcontinuationbet;
            Foldedtoflopcontinuationbet += a.Foldedtoflopcontinuationbet;
            Calledflopcontinuationbet += a.Calledflopcontinuationbet;
            Raisedflopcontinuationbet += a.Raisedflopcontinuationbet;

            FacingflopcontinuationbetIP += a.FacingflopcontinuationbetIP;
            FacingflopcontinuationbetOOP += a.FacingflopcontinuationbetOOP;
            CalledflopcontinuationbetIP += a.CalledflopcontinuationbetIP;
            CalledflopcontinuationbetOOP += a.CalledflopcontinuationbetOOP;
            FoldToFlopcontinuationbetIP += a.FoldToFlopcontinuationbetIP;
            FoldToFlopcontinuationbetOOP += a.FoldToFlopcontinuationbetOOP;

            FoldToThreeBetIP += a.FoldToThreeBetIP;
            FoldToThreeBetOOP += a.FoldToThreeBetOOP;
            FacedThreeBetIP += a.FacedThreeBetIP;
            FacedThreeBetOOP += a.FacedThreeBetOOP;

            FacingFlopContinuationBetFromThreeBetPot += a.FacingFlopContinuationBetFromThreeBetPot;
            FoldedToFlopContinuationBetFromThreeBetPot += a.FoldedToFlopContinuationBetFromThreeBetPot;
            CalledFlopContinuationBetFromThreeBetPot += a.CalledFlopContinuationBetFromThreeBetPot;
            RaisedFlopContinuationBetFromThreeBetPot += a.RaisedFlopContinuationBetFromThreeBetPot;

            FacingFlopContinuationBetFromFourBetPot += a.FacingFlopContinuationBetFromFourBetPot;
            FoldedToFlopContinuationBetFromFourBetPot += a.FoldedToFlopContinuationBetFromFourBetPot;
            CalledFlopContinuationBetFromFourBetPot += a.CalledFlopContinuationBetFromFourBetPot;
            RaisedFlopContinuationBetFromFourBetPot += a.RaisedFlopContinuationBetFromFourBetPot;

            Facingturncontinuationbet += a.Facingturncontinuationbet;
            Foldedtoturncontinuationbet += a.Foldedtoturncontinuationbet;
            Calledturncontinuationbet += a.Calledturncontinuationbet;
            Raisedturncontinuationbet += a.Raisedturncontinuationbet;
            Facingrivercontinuationbet += a.Facingrivercontinuationbet;
            Foldedtorivercontinuationbet += a.Foldedtorivercontinuationbet;
            Calledrivercontinuationbet += a.Calledrivercontinuationbet;
            Raisedrivercontinuationbet += a.Raisedrivercontinuationbet;

            Buttonstealfaced += a.Buttonstealfaced;
            Buttonstealdefended += a.Buttonstealdefended;
            Buttonstealfolded += a.Buttonstealfolded;
            Buttonstealreraised += a.Buttonstealreraised;
            Bigblindstealfaced += a.Bigblindstealfaced;
            Bigblindstealdefended += a.Bigblindstealdefended;
            Bigblindstealreraised += a.Bigblindstealreraised;
            Bigblindstealfolded += a.Bigblindstealfolded;
            Smallblindstealattempted += a.Smallblindstealattempted;
            Smallblindstealfaced += a.Smallblindstealfaced;
            Smallblindstealdefended += a.Smallblindstealdefended;
            Smallblindstealfolded += a.Smallblindstealfolded;
            Smallblindstealreraised += a.Smallblindstealreraised;

            BlindsStealDefended += a.BlindsStealDefended;

            Sawnonsmallshowdown += a.Sawnonsmallshowdown;
            Wonnonsmallshowdown += a.Wonnonsmallshowdown;
            Sawlargeshowdown += a.Sawlargeshowdown;
            Wonlargeshowdown += a.Wonlargeshowdown;
            Sawnonsmallshowdownlimpedflop += a.Sawnonsmallshowdownlimpedflop;
            Wonnonsmallshowdownlimpedflop += a.Wonnonsmallshowdownlimpedflop;
            Sawlargeshowdownlimpedflop += a.Sawlargeshowdownlimpedflop;
            Wonlargeshowdownlimpedflop += a.Wonlargeshowdownlimpedflop;

            Couldfourbet += a.Couldfourbet;
            Didfourbet += a.Didfourbet;
            Facedfourbetpreflop += a.Facedfourbetpreflop;
            Foldedtofourbetpreflop += a.Foldedtofourbetpreflop;
            Calledfourbetpreflop += a.Calledfourbetpreflop;
            Raisedfourbetpreflop += a.Raisedfourbetpreflop;

            Facingtwopreflopraisers += a.Facingtwopreflopraisers;
            Calledtwopreflopraisers += a.Calledtwopreflopraisers;
            Raisedtwopreflopraisers += a.Raisedtwopreflopraisers;

            Turnfoldippassonflopcb += a.Turnfoldippassonflopcb;
            Turncallippassonflopcb += a.Turncallippassonflopcb;
            Turnraiseippassonflopcb += a.Turnraiseippassonflopcb;
            Riverfoldippassonturncb += a.Riverfoldippassonturncb;
            Rivercallippassonturncb += a.Rivercallippassonturncb;
            Riverraiseippassonturncb += a.Riverraiseippassonturncb;

            Couldsqueeze += a.Couldsqueeze;
            Didsqueeze += a.Didsqueeze;

            DidOpenRaise += a.DidOpenRaise;
            IsRaisedLimpers += a.IsRaisedLimpers;
            DidCheckRaise += a.DidCheckRaise;
            DidFlopCheckRaise += a.DidFlopCheckRaise;
            DidTurnCheckRaise += a.DidTurnCheckRaise;
            DidRiverCheckRaise += a.DidRiverCheckRaise;
            CouldFlopCheckRaise += a.CouldFlopCheckRaise;
            CouldTurnCheckRaise += a.CouldTurnCheckRaise;
            CouldRiverCheckRaise += a.CouldRiverCheckRaise;
            FacedFlopCheckRaise += a.FacedFlopCheckRaise;
            FoldedToFlopCheckRaise += a.FoldedToFlopCheckRaise;
            FacedTurnCheckRaise += a.FacedTurnCheckRaise;
            FoldedToTurnCheckRaise += a.FoldedToTurnCheckRaise;
            FacedRiverCheckRaise += a.FacedRiverCheckRaise;
            FoldedToRiverCheckRaise += a.FoldedToRiverCheckRaise;
            CalledTurnCheckRaise += a.CalledTurnCheckRaise;

            CheckedRiverAfterBBLine += a.CheckedRiverAfterBBLine;
            CouldCheckRiverAfterBBLine += a.CouldCheckRiverAfterBBLine;
            DidBetRiverOnBXLine += a.DidBetRiverOnBXLine;
            CouldBetRiverOnBXLine += a.CouldBetRiverOnBXLine;
            DidRiverBet += a.DidRiverBet;
            CouldRiverBet += a.CouldRiverBet;

            CouldTurnBet += a.CouldTurnBet;
            CouldFlopBet += a.CouldFlopBet;

            FoldedFlop += a.FoldedFlop;
            FacedBetOnFlop += a.FacedBetOnFlop;

            IsRelativePosition = a.IsRelativePosition;
            IsRelative3BetPosition = a.IsRelative3BetPosition;

            PlayedFloatFlop += a.PlayedFloatFlop;

            CouldRaiseFlop += a.CouldRaiseFlop;
            CouldRaiseTurn += a.CouldRaiseTurn;
            CouldRaiseRiver += a.CouldRaiseRiver;

            DidRaiseFlop += a.DidRaiseFlop;
            DidRaiseTurn += a.DidRaiseTurn;
            DidRaiseRiver += a.DidRaiseRiver;

            Couldcoldcall += a.Couldcoldcall;
            Didcoldcall += a.Didcoldcall;
            DidColdCallIp += a.DidColdCallIp;
            DidColdCallOop += a.DidColdCallOop;

            SawUnopenedPot += a.SawUnopenedPot;

            UO_PFR_EP += a.UO_PFR_EP;
            UO_PFR_MP += a.UO_PFR_MP;
            UO_PFR_CO += a.UO_PFR_CO;
            UO_PFR_BN += a.UO_PFR_BN;
            UO_PFR_SB += a.UO_PFR_SB;
            UO_PFR_BB += a.UO_PFR_BB;

            DidThreeBetInBb += a.DidThreeBetInBb;
            DidThreeBetInBtn += a.DidThreeBetInBtn;
            DidThreeBetInCo += a.DidThreeBetInCo;
            DidThreeBetInMp += a.DidThreeBetInMp;
            DidThreeBetInSb += a.DidThreeBetInSb;

            DidthreebetBluffInSb += a.DidthreebetBluffInSb;
            DidthreebetBluffInBb += a.DidthreebetBluffInBb;
            DidthreebetBluffInBlinds += a.DidthreebetBluffInBlinds;

            DidFourBetInBb += a.DidFourBetInBb;
            DidFourBetInBtn += a.DidFourBetInBtn;
            DidFourBetInCo += a.DidFourBetInCo;
            DidFourBetInMp += a.DidFourBetInMp;
            DidFourBetInSb += a.DidFourBetInSb;

            DidfourbetBluff += a.DidfourbetBluff;
            DidFourBetBluffInBtn += a.DidFourBetBluffInBtn;

            DidColdCallInBb += a.DidColdCallInBb;
            DidColdCallInBtn += a.DidColdCallInBtn;
            DidColdCallInCo += a.DidColdCallInCo;
            DidColdCallInMp += a.DidColdCallInMp;
            DidColdCallInSb += a.DidColdCallInSb;
            DidColdCallInEp += a.DidColdCallInEp;
            DidColdCallThreeBet += a.DidColdCallThreeBet;
            CouldColdCallThreeBet += a.CouldColdCallThreeBet;
            DidColdCallFourBet += a.DidColdCallFourBet;
            CouldColdCallFourBet += a.CouldColdCallFourBet;
            DidColdCallVsOpenRaiseBtn += a.DidColdCallVsOpenRaiseBtn;
            DidColdCallVsOpenRaiseCo += a.DidColdCallVsOpenRaiseCo;
            DidColdCallVsOpenRaiseSb += a.DidColdCallVsOpenRaiseSb;
            CouldColdCallVsOpenRaiseBtn += a.CouldColdCallVsOpenRaiseBtn;
            CouldColdCallVsOpenRaiseCo += a.CouldColdCallVsOpenRaiseCo;
            CouldColdCallVsOpenRaiseSb += a.CouldColdCallVsOpenRaiseSb;

            FirstRaiser += a.FirstRaiser;

            StealPossible += a.StealPossible;
            StealMade += a.StealMade;

            CouldThreeBetVsSteal += a.CouldThreeBetVsSteal;
            DidThreeBetVsSteal += a.DidThreeBetVsSteal;

            CouldCheckRiverOnBXLine += a.CouldCheckRiverOnBXLine;
            DidCheckRiverOnBXLine += a.DidCheckRiverOnBXLine;

            TotalAggressiveBets += a.TotalAggressiveBets;

            DidBluffedRiver += a.DidBluffedRiver;

            DidCheckFlop += a.DidCheckFlop;

            FacedHandsUpOnFlop += a.FacedHandsUpOnFlop;
            FacedMultiWayOnFlop += a.FacedMultiWayOnFlop;

            SessionCode = a.SessionCode;

            LimpPossible += a.LimpPossible;
            LimpMade += a.LimpMade;
            LimpFaced += a.LimpFaced;
            LimpCalled += a.LimpCalled;
            LimpFolded += a.LimpFolded;
            LimpReraised += a.LimpReraised;
            LimpBtn += a.LimpBtn;
            LimpEp += a.LimpEp;
            LimpMp += a.LimpMp;
            LimpCo += a.LimpCo;
            LimpSb += a.LimpSb;

            TotalbetsFlop += a.TotalbetsFlop;
            TotalbetsTurn += a.TotalbetsTurn;
            TotalbetsRiver += a.TotalbetsRiver;

            TotalcallsFlop += a.TotalcallsFlop;
            TotalcallsTurn += a.TotalcallsTurn;
            TotalcallsRiver += a.TotalcallsRiver;

            FlopAggPossible += a.FlopAggPossible;
            TurnAggPossible += a.TurnAggPossible;
            RiverAggPossible += a.RiverAggPossible;

            CheckFoldFlopPfrOop += a.CheckFoldFlopPfrOop;
            CheckFoldFlop3BetOop += a.CheckFoldFlop3BetOop;
            BetFoldFlopPfrRaiser += a.BetFoldFlopPfrRaiser;
            CouldBetFoldFlopPfrRaiser += a.CouldBetFoldFlopPfrRaiser;
            BetFlopCalled3BetPreflopIp += a.BetFlopCalled3BetPreflopIp;
            CouldBetFlopCalled3BetPreflopIp += a.CouldBetFlopCalled3BetPreflopIp;

            PfrOop += a.PfrOop;
            PfrInEp += a.PfrInEp;
            PfrInMp += a.PfrInMp;
            PfrInCo += a.PfrInCo;
            PfrInBtn += a.PfrInBtn;
            PfrInSb += a.PfrInSb;
            PfrInBb += a.PfrInBb;

            FacedRaiseFlop += a.FacedRaiseFlop;
            FoldedFacedRaiseFlop += a.FoldedFacedRaiseFlop;
            CalledFacedRaiseFlop += a.CalledFacedRaiseFlop;
            ReraisedFacedRaiseFlop += a.ReraisedFacedRaiseFlop;
            FacedRaiseTurn += a.FacedRaiseTurn;
            FoldedFacedRaiseTurn += a.FoldedFacedRaiseTurn;
            CalledFacedRaiseTurn += a.CalledFacedRaiseTurn;
            ReraisedFacedRaiseTurn += a.ReraisedFacedRaiseTurn;
            FacedRaiseRiver += a.FacedRaiseRiver;
            FoldedFacedRaiseRiver += a.FoldedFacedRaiseRiver;
            CalledFacedRaiseRiver += a.CalledFacedRaiseRiver;
            ReraisedFacedRaiseRiver += a.ReraisedFacedRaiseRiver;
            CanBetWhenCheckedToFlop += a.CanBetWhenCheckedToFlop;
            DidBetWhenCheckedToFlop += a.DidBetWhenCheckedToFlop;
            CanBetWhenCheckedToTurn += a.CanBetWhenCheckedToTurn;
            DidBetWhenCheckedToTurn += a.DidBetWhenCheckedToTurn;
            CanBetWhenCheckedToRiver += a.CanBetWhenCheckedToRiver;
            DidBetWhenCheckedToRiver += a.DidBetWhenCheckedToRiver;
            FacedSqueez += a.FacedSqueez;
            FoldedFacedSqueez += a.FoldedFacedSqueez;
            CalledFacedSqueez += a.CalledFacedSqueez;
            ReraisedFacedSqueez += a.ReraisedFacedSqueez;

            MRatio = a.MRatio;
            StackInBBs = a.StackInBBs;
            EVDiff = a.EVDiff;

            PreflopIP = a.PreflopIP;
            PreflopOOP = a.PreflopOOP;

            #region tilt meter 

            CalculateTiltMeterValue(this, a);

            #endregion

            DidDelayedTurnCBet += a.DidDelayedTurnCBet;
            CouldDelayedTurnCBet += a.CouldDelayedTurnCBet;
            DidDelayedTurnCBetIn3BetPot += a.DidDelayedTurnCBetIn3BetPot;
            CouldDelayedTurnCBetIn3BetPot += a.CouldDelayedTurnCBetIn3BetPot;

            DidDonkBet += a.DidDonkBet;
            CouldDonkBet += a.CouldDonkBet;
            FacedDonkBet += a.FacedDonkBet;
            FoldedToDonkBet += a.FoldedToDonkBet;

            FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP += a.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP;
            CheckedCalledTurnWhenCheckedFlopAsPfr += a.CheckedCalledTurnWhenCheckedFlopAsPfr;
            CheckedFoldedToTurnWhenCheckedFlopAsPfr += a.CheckedFoldedToTurnWhenCheckedFlopAsPfr;
            FacedTurnBetWhenCheckedFlopAsPfr += a.FacedTurnBetWhenCheckedFlopAsPfr;
            CalledTurnBetWhenCheckedFlopAsPfr += a.CalledTurnBetWhenCheckedFlopAsPfr;
            FoldedToTurnBetWhenCheckedFlopAsPfr += a.FoldedToTurnBetWhenCheckedFlopAsPfr;
            RaisedTurnBetWhenCheckedFlopAsPfr += a.RaisedTurnBetWhenCheckedFlopAsPfr;

            FlopBetToPotRatio = a.FlopBetToPotRatio;
            TurnBetToPotRatio = a.TurnBetToPotRatio;
            RiverBetToPotRatio = a.RiverBetToPotRatio;

            DidFlopCheckBehind += a.DidFlopCheckBehind;
            CouldFlopCheckBehind += a.CouldFlopCheckBehind;

            FoldedTurn += a.FoldedTurn;
            FacedBetOnTurn += a.FacedBetOnTurn;

            CheckedCalledRiver += a.CheckedCalledRiver;
            CheckedFoldedRiver += a.CheckedFoldedRiver;
            CheckedThenFacedBetOnRiver += a.CheckedThenFacedBetOnRiver;

            RiverWonOnFacingBet += a.RiverWonOnFacingBet;
            RiverCallSizeOnFacingBet += a.RiverCallSizeOnFacingBet;

            ShovedFlopAfter4Bet += a.ShovedFlopAfter4Bet;
            CouldShoveFlopAfter4Bet += a.CouldShoveFlopAfter4Bet;

            BetFlopWhenCheckedToSRP += a.BetFlopWhenCheckedToSRP;
            CouldBetFlopWhenCheckedToSRP += a.CouldBetFlopWhenCheckedToSRP;

            FacedBetOnRiver += a.FacedBetOnRiver;
            RiverVsBetFold += a.RiverVsBetFold;
            FlopCBetSuccess += a.FlopCBetSuccess;
            DidCheckTurn += a.DidCheckTurn;
            TotalCallAmountOnRiver += a.TotalCallAmountOnRiver;
            TotalWonAmountOnRiverCall += a.TotalWonAmountOnRiverCall;

            FirstRaiserPosition = a.FirstRaiserPosition;
            ThreeBettorPosition = a.ThreeBettorPosition;
            CouldProbeBetTurn += a.CouldProbeBetTurn;
            CouldProbeBetRiver += a.CouldProbeBetRiver;

            PreflopRaisersCount = a.PreflopRaisersCount;
            OpenRaisePreflopInBBs = a.OpenRaisePreflopInBBs;

            FacedDelayedCBet += a.FacedDelayedCBet;
        }

        public static Playerstatistic operator +(Playerstatistic a, Playerstatistic b)
        {
            Playerstatistic r = new Playerstatistic();
            r.GameNumber = b.GameNumber;
            r.PokersiteId = b.PokersiteId;
            r.Time = b.Time;
            r.Pot = b.Pot;
            r.Numberofplayers = b.Numberofplayers;
            r.GametypeId = b.GametypeId;
            r.PlayerId = b.PlayerId;
            r.PlayerName = b.PlayerName;
            r.Playedyearandmonth = b.Playedyearandmonth;
            r.Position = b.Position;
            r.PositionString = b.PositionString;
            r.Allin = b.Allin;
            r.Line = b.Line;
            r.FacingPreflop = b.FacingPreflop;
            r.GameType = b.GameType;
            r.Stakes = b.GameType;
            r.IsTourney = b.IsTourney;
            r.SmallBlind = b.SmallBlind;
            r.BigBlind = b.BigBlind;
            r.StartingStack = b.StartingStack;
            r.Ante = b.Ante;
            r.IsRelativePosition = b.IsRelativePosition;
            r.IsRelative3BetPosition = b.IsRelative3BetPosition;
            r.CurrencyId = b.CurrencyId;
            r.PokergametypeId = b.PokergametypeId;
            r.TotalPot = a.TotalPot + b.TotalPot;
            r.TotalPotInBB = a.TotalPotInBB + b.TotalPotInBB;
            r.LimpBtn = a.LimpBtn + b.LimpBtn;
            r.LimpEp = a.LimpEp + b.LimpEp;
            r.LimpMp = a.LimpMp + b.LimpMp;
            r.LimpCo = a.LimpCo + b.LimpCo;
            r.LimpSb = a.LimpSb + b.LimpSb;
            r.Sawshowdown = a.Sawshowdown + b.Sawshowdown;
            r.Sawflop = a.Sawflop + b.Sawflop;
            r.SawTurn = a.SawTurn + b.SawTurn;
            r.SawRiver = a.SawRiver + b.SawRiver;

            r.WasFlop = a.WasFlop + b.WasFlop;
            r.WasTurn = a.WasTurn + b.WasTurn;
            r.WasRiver = a.WasRiver + b.WasRiver;

            r.Wonshowdown = a.Wonshowdown + b.Wonshowdown;
            r.Wonhandwhensawflop = a.Wonhandwhensawflop + b.Wonhandwhensawflop;
            r.Wonhandwhensawturn = a.Wonhandwhensawturn + b.Wonhandwhensawturn;
            r.Wonhandwhensawriver = a.Wonhandwhensawriver + b.Wonhandwhensawriver;
            r.Wonhand = a.Wonhand + b.Wonhand;

            r.Vpiphands = a.Vpiphands + b.Vpiphands;
            r.Pfrhands = a.Pfrhands + b.Pfrhands;

            r.Couldthreebet = a.Couldthreebet + b.Couldthreebet;
            r.Didthreebet = a.Didthreebet + b.Didthreebet;
            r.DidThreeBetIp = a.DidThreeBetIp + b.DidThreeBetIp;
            r.CouldThreeBetIp = a.CouldThreeBetIp + b.CouldThreeBetIp;
            r.DidThreeBetOop = a.DidThreeBetOop + b.DidThreeBetOop;
            r.CouldThreeBetOop = a.CouldThreeBetOop + b.CouldThreeBetOop;
            r.Facedthreebetpreflop = a.Facedthreebetpreflop + b.Facedthreebetpreflop;
            r.Foldedtothreebetpreflop = a.Foldedtothreebetpreflop + b.Foldedtothreebetpreflop;
            r.Calledthreebetpreflop = a.Calledthreebetpreflop + b.Calledthreebetpreflop;
            r.Raisedthreebetpreflop = a.Raisedthreebetpreflop + b.Raisedthreebetpreflop;

            r.Totalbbswon = a.Totalbbswon + b.Totalbbswon;
            r.Totalhands = a.Totalhands + b.Totalhands;
            r.Totalbets = a.Totalbets + b.Totalbets;
            r.Totalcalls = a.Totalcalls + b.Totalcalls;
            r.Totalpostflopstreetsplayed = a.Totalpostflopstreetsplayed + b.Totalpostflopstreetsplayed;
            r.Totalamountwonincents = a.Totalamountwonincents + b.Totalamountwonincents;
            r.Totalaggressivepostflopstreetsseen = a.Totalaggressivepostflopstreetsseen + b.Totalaggressivepostflopstreetsseen;
            r.Totalrakeincents = a.Totalrakeincents + b.Totalrakeincents;
            r.NumberOfWalks = a.NumberOfWalks + b.NumberOfWalks;

            r.Flopcontinuationbetpossible = a.Flopcontinuationbetpossible + b.Flopcontinuationbetpossible;
            r.Flopcontinuationbetmade = a.Flopcontinuationbetmade + b.Flopcontinuationbetmade;
            r.Flopcontinuationipbetmade = a.Flopcontinuationipbetmade + b.Flopcontinuationipbetmade;
            r.Flopcontinuationipbetpossible = a.Flopcontinuationipbetpossible + b.Flopcontinuationipbetpossible;
            r.Flopcontinuationoopbetmade = a.Flopcontinuationoopbetmade + b.Flopcontinuationoopbetmade;
            r.Flopcontinuationoopbetpossible = a.Flopcontinuationoopbetpossible + b.Flopcontinuationoopbetpossible;
            r.FlopContinuationBetInThreeBetPotMade = a.FlopContinuationBetInThreeBetPotMade + b.FlopContinuationBetInThreeBetPotMade;
            r.FlopContinuationBetInThreeBetPotPossible = a.FlopContinuationBetInThreeBetPotPossible + b.FlopContinuationBetInThreeBetPotPossible;
            r.FlopContinuationBetInFourBetPotMade = a.FlopContinuationBetInFourBetPotMade + b.FlopContinuationBetInFourBetPotMade;
            r.FlopContinuationBetInFourBetPotPossible = a.FlopContinuationBetInFourBetPotPossible + b.FlopContinuationBetInFourBetPotPossible;
            r.FlopContinuationBetVsOneOpponentMade = a.FlopContinuationBetVsOneOpponentMade + b.FlopContinuationBetVsOneOpponentMade;
            r.FlopContinuationBetVsOneOpponentPossible = a.FlopContinuationBetVsOneOpponentPossible + b.FlopContinuationBetVsOneOpponentPossible;
            r.FlopContinuationBetVsTwoOpponentsMade = a.FlopContinuationBetVsTwoOpponentsMade + b.FlopContinuationBetVsTwoOpponentsMade;
            r.FlopContinuationBetVsTwoOpponentsPossible = a.FlopContinuationBetVsTwoOpponentsPossible + b.FlopContinuationBetVsTwoOpponentsPossible;
            r.MultiWayFlopContinuationBetMade = a.MultiWayFlopContinuationBetMade + b.MultiWayFlopContinuationBetMade;
            r.MultiWayFlopContinuationBetPossible = a.MultiWayFlopContinuationBetPossible + b.MultiWayFlopContinuationBetPossible;
            r.FlopContinuationBetMonotonePotMade = a.FlopContinuationBetMonotonePotMade + b.FlopContinuationBetMonotonePotMade;
            r.FlopContinuationBetMonotonePotPossible = a.FlopContinuationBetMonotonePotPossible + b.FlopContinuationBetMonotonePotPossible;
            r.FlopContinuationBetRagPotMade = a.FlopContinuationBetRagPotMade + b.FlopContinuationBetRagPotMade;
            r.FlopContinuationBetRagPotPossible = a.FlopContinuationBetRagPotPossible + b.FlopContinuationBetRagPotPossible;

            r.Turncontinuationbetpossible = a.Turncontinuationbetpossible + b.Turncontinuationbetpossible;
            r.Turncontinuationbetmade = a.Turncontinuationbetmade + b.Turncontinuationbetmade;
            r.Rivercontinuationbetpossible = a.Rivercontinuationbetpossible + b.Rivercontinuationbetpossible;
            r.Rivercontinuationbetmade = a.Rivercontinuationbetmade + b.Rivercontinuationbetmade;

            r.TurnContinuationBetWithAirMade = a.TurnContinuationBetWithAirMade + b.TurnContinuationBetWithAirMade;

            r.Facingflopcontinuationbet = a.Facingflopcontinuationbet + b.Facingflopcontinuationbet;
            r.Foldedtoflopcontinuationbet = a.Foldedtoflopcontinuationbet + b.Foldedtoflopcontinuationbet;
            r.Calledflopcontinuationbet = a.Calledflopcontinuationbet + b.Calledflopcontinuationbet;
            r.Raisedflopcontinuationbet = a.Raisedflopcontinuationbet + b.Raisedflopcontinuationbet;

            r.FacingflopcontinuationbetIP = a.FacingflopcontinuationbetIP + b.FacingflopcontinuationbetIP;
            r.FacingflopcontinuationbetOOP = a.FacingflopcontinuationbetOOP + b.FacingflopcontinuationbetOOP;
            r.CalledflopcontinuationbetIP = a.CalledflopcontinuationbetIP + b.CalledflopcontinuationbetIP;
            r.CalledflopcontinuationbetOOP = a.CalledflopcontinuationbetOOP + b.CalledflopcontinuationbetOOP;
            r.FoldToFlopcontinuationbetIP = a.FoldToFlopcontinuationbetIP + b.FoldToFlopcontinuationbetIP;
            r.FoldToFlopcontinuationbetOOP = a.FoldToFlopcontinuationbetOOP + b.FoldToFlopcontinuationbetOOP;

            r.FoldToThreeBetIP = a.FoldToThreeBetIP + b.FoldToThreeBetIP;
            r.FoldToThreeBetOOP = a.FoldToThreeBetOOP + b.FoldToThreeBetOOP;
            r.FacedThreeBetIP = a.FacedThreeBetIP + b.FacedThreeBetIP;
            r.FacedThreeBetOOP = a.FacedThreeBetOOP + b.FacedThreeBetOOP;

            r.FacingFlopContinuationBetFromThreeBetPot = a.FacingFlopContinuationBetFromThreeBetPot + b.FacingFlopContinuationBetFromThreeBetPot;
            r.FoldedToFlopContinuationBetFromThreeBetPot = a.FoldedToFlopContinuationBetFromThreeBetPot + b.FoldedToFlopContinuationBetFromThreeBetPot;
            r.CalledFlopContinuationBetFromThreeBetPot = a.CalledFlopContinuationBetFromThreeBetPot + b.CalledFlopContinuationBetFromThreeBetPot;
            r.RaisedFlopContinuationBetFromThreeBetPot = a.RaisedFlopContinuationBetFromThreeBetPot + b.RaisedFlopContinuationBetFromThreeBetPot;

            r.FacingFlopContinuationBetFromFourBetPot = a.FacingFlopContinuationBetFromFourBetPot + b.FacingFlopContinuationBetFromFourBetPot;
            r.FoldedToFlopContinuationBetFromFourBetPot = a.FoldedToFlopContinuationBetFromFourBetPot + b.FoldedToFlopContinuationBetFromFourBetPot;
            r.CalledFlopContinuationBetFromFourBetPot = a.CalledFlopContinuationBetFromFourBetPot + b.CalledFlopContinuationBetFromFourBetPot;
            r.RaisedFlopContinuationBetFromFourBetPot = a.RaisedFlopContinuationBetFromFourBetPot + b.RaisedFlopContinuationBetFromFourBetPot;

            r.Facingturncontinuationbet = a.Facingturncontinuationbet + b.Facingturncontinuationbet;
            r.Foldedtoturncontinuationbet = a.Foldedtoturncontinuationbet + b.Foldedtoturncontinuationbet;
            r.Calledturncontinuationbet = a.Calledturncontinuationbet + b.Calledturncontinuationbet;
            r.Raisedturncontinuationbet = a.Raisedturncontinuationbet + b.Raisedturncontinuationbet;
            r.Facingrivercontinuationbet = a.Facingrivercontinuationbet + b.Facingrivercontinuationbet;
            r.Foldedtorivercontinuationbet = a.Foldedtorivercontinuationbet + b.Foldedtorivercontinuationbet;
            r.Calledrivercontinuationbet = a.Calledrivercontinuationbet + b.Calledrivercontinuationbet;
            r.Raisedrivercontinuationbet = a.Raisedrivercontinuationbet + b.Raisedrivercontinuationbet;

            r.Buttonstealfaced = a.Buttonstealfaced + b.Buttonstealfaced;
            r.Buttonstealdefended = a.Buttonstealdefended + b.Buttonstealdefended;
            r.Buttonstealfolded = a.Buttonstealfolded + b.Buttonstealfolded;
            r.Buttonstealreraised = a.Buttonstealreraised + b.Buttonstealreraised;
            r.Bigblindstealfaced = a.Bigblindstealfaced + b.Bigblindstealfaced;
            r.Bigblindstealdefended = a.Bigblindstealdefended + b.Bigblindstealdefended;
            r.Bigblindstealreraised = a.Bigblindstealreraised + b.Bigblindstealreraised;
            r.Bigblindstealfolded = a.Bigblindstealfolded + b.Bigblindstealfolded;
            r.Smallblindstealattempted = a.Smallblindstealattempted + b.Smallblindstealattempted;
            r.Smallblindstealfaced = a.Smallblindstealfaced + b.Smallblindstealfaced;
            r.Smallblindstealdefended = a.Smallblindstealdefended + b.Smallblindstealdefended;
            r.Smallblindstealfolded = a.Smallblindstealfolded + b.Smallblindstealfolded;
            r.Smallblindstealreraised = a.Smallblindstealreraised + b.Smallblindstealreraised;

            r.BlindsStealDefended = a.BlindsStealDefended + b.BlindsStealDefended;

            r.Sawnonsmallshowdown = a.Sawnonsmallshowdown + b.Sawnonsmallshowdown;
            r.Wonnonsmallshowdown = a.Wonnonsmallshowdown + b.Wonnonsmallshowdown;
            r.Sawlargeshowdown = a.Sawlargeshowdown + b.Sawlargeshowdown;
            r.Wonlargeshowdown = a.Wonlargeshowdown + b.Wonlargeshowdown;
            r.Sawnonsmallshowdownlimpedflop = a.Sawnonsmallshowdownlimpedflop + b.Sawnonsmallshowdownlimpedflop;
            r.Wonnonsmallshowdownlimpedflop = a.Wonnonsmallshowdownlimpedflop + b.Wonnonsmallshowdownlimpedflop;
            r.Sawlargeshowdownlimpedflop = a.Sawlargeshowdownlimpedflop + b.Sawlargeshowdownlimpedflop;
            r.Wonlargeshowdownlimpedflop = a.Wonlargeshowdownlimpedflop + b.Wonlargeshowdownlimpedflop;

            r.Couldfourbet = a.Couldfourbet + b.Couldfourbet;
            r.Didfourbet = a.Didfourbet + b.Didfourbet;
            r.Facedfourbetpreflop = a.Facedfourbetpreflop + b.Facedfourbetpreflop;
            r.Foldedtofourbetpreflop = a.Foldedtofourbetpreflop + b.Foldedtofourbetpreflop;
            r.Calledfourbetpreflop = a.Calledfourbetpreflop + b.Calledfourbetpreflop;
            r.Raisedfourbetpreflop = a.Raisedfourbetpreflop + b.Raisedfourbetpreflop;

            r.Did5Bet = a.Did5Bet + b.Did5Bet;
            r.Could5Bet = a.Could5Bet + b.Could5Bet;
            r.Faced5Bet = a.Faced5Bet + b.Faced5Bet;
            r.FoldedTo5Bet = a.FoldedTo5Bet + b.FoldedTo5Bet;

            r.CalledCheckRaiseVsFlopCBet = a.CalledCheckRaiseVsFlopCBet + b.CalledCheckRaiseVsFlopCBet;
            r.FoldedCheckRaiseVsFlopCBet = a.FoldedCheckRaiseVsFlopCBet + b.FoldedCheckRaiseVsFlopCBet;
            r.FacedCheckRaiseVsFlopCBet = a.FacedCheckRaiseVsFlopCBet + b.FacedCheckRaiseVsFlopCBet;

            r.Facingtwopreflopraisers = a.Facingtwopreflopraisers + b.Facingtwopreflopraisers;
            r.Calledtwopreflopraisers = a.Calledtwopreflopraisers + b.Calledtwopreflopraisers;
            r.Raisedtwopreflopraisers = a.Raisedtwopreflopraisers + b.Raisedtwopreflopraisers;

            r.Turnfoldippassonflopcb = a.Turnfoldippassonflopcb + b.Turnfoldippassonflopcb;
            r.Turncallippassonflopcb = a.Turncallippassonflopcb + b.Turncallippassonflopcb;
            r.Turnraiseippassonflopcb = a.Turnraiseippassonflopcb + b.Turnraiseippassonflopcb;
            r.Riverfoldippassonturncb = a.Riverfoldippassonturncb + b.Riverfoldippassonturncb;
            r.Rivercallippassonturncb = a.Rivercallippassonturncb + b.Rivercallippassonturncb;
            r.Riverraiseippassonturncb = a.Riverraiseippassonturncb + b.Riverraiseippassonturncb;

            r.Couldsqueeze = a.Couldsqueeze + b.Couldsqueeze;
            r.DidOpenRaise = a.DidOpenRaise + b.DidOpenRaise;
            r.Didsqueeze = a.Didsqueeze + b.Didsqueeze;

            r.DidCheckRaise = a.DidCheckRaise + b.DidCheckRaise;
            r.DidFlopCheckRaise = a.DidFlopCheckRaise + b.DidFlopCheckRaise;
            r.DidTurnCheckRaise = a.DidTurnCheckRaise + b.DidTurnCheckRaise;
            r.DidRiverCheckRaise = a.DidRiverCheckRaise + b.DidRiverCheckRaise;
            r.CouldFlopCheckRaise = a.CouldFlopCheckRaise + b.CouldFlopCheckRaise;
            r.CouldTurnCheckRaise = a.CouldTurnCheckRaise + b.CouldTurnCheckRaise;
            r.CouldRiverCheckRaise = a.CouldRiverCheckRaise + b.CouldRiverCheckRaise;
            r.FacedFlopCheckRaise = a.FacedFlopCheckRaise + b.FacedFlopCheckRaise;
            r.FoldedToFlopCheckRaise = a.FoldedToFlopCheckRaise + b.FoldedToFlopCheckRaise;
            r.FacedTurnCheckRaise = a.FacedTurnCheckRaise + b.FacedTurnCheckRaise;
            r.FoldedToTurnCheckRaise = a.FoldedToTurnCheckRaise + b.FoldedToTurnCheckRaise;
            r.FacedRiverCheckRaise = a.FacedRiverCheckRaise + b.FacedRiverCheckRaise;
            r.FoldedToRiverCheckRaise = a.FoldedToRiverCheckRaise + b.FoldedToRiverCheckRaise;
            r.CalledTurnCheckRaise = a.CalledTurnCheckRaise + b.CalledTurnCheckRaise;

            r.CheckedRiverAfterBBLine = a.CheckedRiverAfterBBLine + b.CheckedRiverAfterBBLine;
            r.CouldCheckRiverAfterBBLine = a.CouldCheckRiverAfterBBLine + b.CouldCheckRiverAfterBBLine;
            r.DidBetRiverOnBXLine = a.DidBetRiverOnBXLine + b.DidBetRiverOnBXLine;
            r.CouldBetRiverOnBXLine = a.CouldBetRiverOnBXLine + b.CouldBetRiverOnBXLine;
            r.DidRiverBet = a.DidRiverBet + b.DidRiverBet;
            r.CouldRiverBet = a.CouldRiverBet + b.CouldRiverBet;

            r.CouldTurnBet = a.CouldTurnBet + b.CouldTurnBet;
            r.CouldFlopBet = a.CouldFlopBet + b.CouldFlopBet;

            r.FoldedFlop = a.FoldedFlop + b.FoldedFlop;
            r.FacedBetOnFlop = a.FacedBetOnFlop + b.FacedBetOnFlop;

            r.IsRaisedLimpers = a.IsRaisedLimpers + b.IsRaisedLimpers;
            r.SawUnopenedPot = a.SawUnopenedPot + b.SawUnopenedPot;

            r.Couldcoldcall = a.Couldcoldcall + b.Couldcoldcall;
            r.Didcoldcall = a.Didcoldcall + b.Didcoldcall;
            r.DidColdCallIp = a.DidColdCallIp + b.DidColdCallIp;
            r.DidColdCallOop = a.DidColdCallOop + b.DidColdCallOop;

            r.PlayedFloatFlop = a.PlayedFloatFlop + b.PlayedFloatFlop;

            r.CouldRaiseFlop = a.CouldRaiseFlop + b.CouldRaiseFlop;
            r.CouldRaiseTurn = a.CouldRaiseTurn + b.CouldRaiseTurn;
            r.CouldRaiseRiver = a.CouldRaiseRiver + b.CouldRaiseRiver;

            r.DidRaiseFlop = a.DidRaiseFlop + b.DidRaiseFlop;
            r.DidRaiseTurn = a.DidRaiseTurn + b.DidRaiseTurn;
            r.DidRaiseRiver = a.DidRaiseRiver + b.DidRaiseRiver;

            r.UO_PFR_EP = a.UO_PFR_EP + b.UO_PFR_EP;
            r.UO_PFR_MP = a.UO_PFR_MP + b.UO_PFR_MP;
            r.UO_PFR_CO = a.UO_PFR_CO + b.UO_PFR_CO;
            r.UO_PFR_BN = a.UO_PFR_BN + b.UO_PFR_BN;
            r.UO_PFR_SB = a.UO_PFR_SB + b.UO_PFR_SB;
            r.UO_PFR_BB = a.UO_PFR_BB + b.UO_PFR_BB;

            r.DidThreeBetInBb = a.DidThreeBetInBb + b.DidThreeBetInBb;
            r.DidThreeBetInBtn = a.DidThreeBetInBtn + b.DidThreeBetInBtn;
            r.DidThreeBetInCo = a.DidThreeBetInCo + b.DidThreeBetInCo;
            r.DidThreeBetInMp = a.DidThreeBetInMp + b.DidThreeBetInMp;
            r.DidThreeBetInSb = a.DidThreeBetInSb + b.DidThreeBetInSb;

            r.DidthreebetBluffInSb = a.DidthreebetBluffInSb + b.DidthreebetBluffInSb;
            r.DidthreebetBluffInBb = a.DidthreebetBluffInBb + b.DidthreebetBluffInBb;
            r.DidthreebetBluffInBlinds = a.DidthreebetBluffInBlinds + b.DidthreebetBluffInBlinds;

            r.DidFourBetInBb = a.DidFourBetInBb + b.DidFourBetInBb;
            r.DidFourBetInBtn = a.DidFourBetInBtn + b.DidFourBetInBtn;
            r.DidFourBetInCo = a.DidFourBetInCo + b.DidFourBetInCo;
            r.DidFourBetInMp = a.DidFourBetInMp + b.DidFourBetInMp;
            r.DidFourBetInSb = a.DidFourBetInSb + b.DidFourBetInSb;

            r.DidfourbetBluff = a.DidfourbetBluff + b.DidfourbetBluff;
            r.DidFourBetBluffInBtn = a.DidFourBetBluffInBtn + b.DidFourBetBluffInBtn;

            r.DidColdCallInBb = a.DidColdCallInBb + b.DidColdCallInBb;
            r.DidColdCallInBtn = a.DidColdCallInBtn + b.DidColdCallInBtn;
            r.DidColdCallInCo = a.DidColdCallInCo + b.DidColdCallInCo;
            r.DidColdCallInMp = a.DidColdCallInMp + b.DidColdCallInMp;
            r.DidColdCallInSb = a.DidColdCallInSb + b.DidColdCallInSb;
            r.DidColdCallInEp = a.DidColdCallInEp + b.DidColdCallInEp;
            r.DidColdCallThreeBet = a.DidColdCallThreeBet + b.DidColdCallThreeBet;
            r.CouldColdCallThreeBet = a.CouldColdCallThreeBet + b.CouldColdCallThreeBet;
            r.DidColdCallFourBet = a.DidColdCallFourBet + b.DidColdCallFourBet;
            r.CouldColdCallFourBet = a.CouldColdCallFourBet + b.CouldColdCallFourBet;
            r.DidColdCallVsOpenRaiseBtn = a.DidColdCallVsOpenRaiseBtn + b.DidColdCallVsOpenRaiseBtn;
            r.DidColdCallVsOpenRaiseCo = a.DidColdCallVsOpenRaiseCo + b.DidColdCallVsOpenRaiseCo;
            r.DidColdCallVsOpenRaiseSb = a.DidColdCallVsOpenRaiseSb + b.DidColdCallVsOpenRaiseSb;
            r.CouldColdCallVsOpenRaiseBtn = a.CouldColdCallVsOpenRaiseBtn + b.CouldColdCallVsOpenRaiseBtn;
            r.CouldColdCallVsOpenRaiseCo = a.CouldColdCallVsOpenRaiseCo + b.CouldColdCallVsOpenRaiseCo;
            r.CouldColdCallVsOpenRaiseSb = a.CouldColdCallVsOpenRaiseSb + b.CouldColdCallVsOpenRaiseSb;

            r.FirstRaiser = a.FirstRaiser + b.FirstRaiser;

            r.StealPossible = a.StealPossible + b.StealPossible;
            r.StealMade = a.StealMade + b.StealMade;

            r.TournamentId = b.TournamentId;

            r.PFRInRow = a.PFRInRow;
            r.DidThreeBetInRow = a.DidThreeBetInRow;

            r.CouldThreeBetVsSteal = a.CouldThreeBetVsSteal + b.CouldThreeBetVsSteal;
            r.DidThreeBetVsSteal = a.DidThreeBetVsSteal + b.DidThreeBetVsSteal;

            r.CouldCheckRiverOnBXLine = a.CouldCheckRiverOnBXLine + b.CouldCheckRiverOnBXLine;
            r.DidCheckRiverOnBXLine = a.DidCheckRiverOnBXLine + b.DidCheckRiverOnBXLine;

            r.TotalAggressiveBets = a.TotalAggressiveBets + b.TotalAggressiveBets;

            r.DidBluffedRiver = a.DidBluffedRiver + b.DidBluffedRiver;

            r.DidCheckFlop = a.DidCheckFlop + b.DidCheckFlop;

            r.FacedHandsUpOnFlop = a.FacedHandsUpOnFlop + b.FacedHandsUpOnFlop;
            r.FacedMultiWayOnFlop = a.FacedMultiWayOnFlop + b.FacedMultiWayOnFlop;

            r.SessionCode = b.SessionCode;

            r.LimpPossible = a.LimpPossible + b.LimpPossible;
            r.LimpMade = a.LimpMade + b.LimpMade;
            r.LimpFaced = a.LimpFaced + b.LimpFaced;
            r.LimpCalled = a.LimpCalled + b.LimpCalled;
            r.LimpFolded = a.LimpFolded + b.LimpFolded;
            r.LimpReraised = a.LimpReraised + b.LimpReraised;

            r.TotalbetsFlop = a.TotalbetsFlop + b.TotalbetsFlop;
            r.TotalbetsTurn = a.TotalbetsTurn + b.TotalbetsTurn;
            r.TotalbetsRiver = a.TotalbetsRiver + b.TotalbetsRiver;

            r.TotalcallsFlop = a.TotalcallsFlop + b.TotalcallsFlop;
            r.TotalcallsTurn = a.TotalcallsTurn + b.TotalcallsTurn;
            r.TotalcallsRiver = a.TotalcallsRiver + b.TotalcallsRiver;

            r.FlopAggPossible = a.FlopAggPossible + b.FlopAggPossible;
            r.TurnAggPossible = a.TurnAggPossible + b.TurnAggPossible;
            r.RiverAggPossible = a.RiverAggPossible + b.RiverAggPossible;

            r.CheckFoldFlopPfrOop = a.CheckFoldFlopPfrOop + b.CheckFoldFlopPfrOop;
            r.CouldBetFoldFlopPfrRaiser = a.CouldBetFoldFlopPfrRaiser + b.CouldBetFoldFlopPfrRaiser;
            r.CheckFoldFlop3BetOop = a.CheckFoldFlop3BetOop + b.CheckFoldFlop3BetOop;
            r.BetFoldFlopPfrRaiser = a.BetFoldFlopPfrRaiser + b.BetFoldFlopPfrRaiser;
            r.BetFlopCalled3BetPreflopIp = a.BetFlopCalled3BetPreflopIp + b.BetFlopCalled3BetPreflopIp;
            r.CouldBetFlopCalled3BetPreflopIp = a.CouldBetFlopCalled3BetPreflopIp + b.CouldBetFlopCalled3BetPreflopIp;
            r.PfrOop = a.PfrOop + b.PfrOop;
            r.PfrInEp = a.PfrInEp + b.PfrInEp;
            r.PfrInMp = a.PfrInMp + b.PfrInMp;
            r.PfrInCo = a.PfrInCo + b.PfrInCo;
            r.PfrInBtn = a.PfrInBtn + b.PfrInBtn;
            r.PfrInSb = a.PfrInSb + b.PfrInSb;
            r.PfrInBb = a.PfrInBb + b.PfrInBb;

            r.FacedRaiseFlop = a.FacedRaiseFlop + b.FacedRaiseFlop;
            r.FoldedFacedRaiseFlop = a.FoldedFacedRaiseFlop + b.FoldedFacedRaiseFlop;
            r.CalledFacedRaiseFlop = a.CalledFacedRaiseFlop + b.CalledFacedRaiseFlop;
            r.ReraisedFacedRaiseFlop = a.ReraisedFacedRaiseFlop + b.ReraisedFacedRaiseFlop;
            r.FacedRaiseTurn = a.FacedRaiseTurn + b.FacedRaiseTurn;
            r.FoldedFacedRaiseTurn = a.FoldedFacedRaiseTurn + b.FoldedFacedRaiseTurn;
            r.CalledFacedRaiseTurn = a.CalledFacedRaiseTurn + b.CalledFacedRaiseTurn;
            r.ReraisedFacedRaiseTurn = a.ReraisedFacedRaiseTurn + b.ReraisedFacedRaiseTurn;
            r.FacedRaiseRiver = a.FacedRaiseRiver + b.FacedRaiseRiver;
            r.FoldedFacedRaiseRiver = a.FoldedFacedRaiseRiver + b.FoldedFacedRaiseRiver;
            r.CalledFacedRaiseRiver = a.CalledFacedRaiseRiver + b.CalledFacedRaiseRiver;
            r.ReraisedFacedRaiseRiver = a.ReraisedFacedRaiseRiver + b.ReraisedFacedRaiseRiver;
            r.CanBetWhenCheckedToFlop = a.CanBetWhenCheckedToFlop + b.CanBetWhenCheckedToFlop;
            r.DidBetWhenCheckedToFlop = a.DidBetWhenCheckedToFlop + b.DidBetWhenCheckedToFlop;
            r.CanBetWhenCheckedToTurn = a.CanBetWhenCheckedToTurn + b.CanBetWhenCheckedToTurn;
            r.DidBetWhenCheckedToTurn = a.DidBetWhenCheckedToTurn + b.DidBetWhenCheckedToTurn;
            r.CanBetWhenCheckedToRiver = a.CanBetWhenCheckedToRiver + b.CanBetWhenCheckedToRiver;
            r.DidBetWhenCheckedToRiver = a.DidBetWhenCheckedToRiver + b.DidBetWhenCheckedToRiver;
            r.FacedSqueez = a.FacedSqueez + b.FacedSqueez;
            r.FoldedFacedSqueez = a.FoldedFacedSqueez + b.FoldedFacedSqueez;
            r.CalledFacedSqueez = a.CalledFacedSqueez + b.CalledFacedSqueez;
            r.ReraisedFacedSqueez = a.ReraisedFacedSqueez + b.ReraisedFacedSqueez;

            r.MRatio = b.MRatio;
            r.StackInBBs = b.StackInBBs;

            r.EVDiff = a.EVDiff + b.EVDiff;

            r.PreflopIP = a.PreflopIP + b.PreflopIP;
            r.PreflopOOP = a.PreflopOOP + b.PreflopOOP;

            #region tilt meter

            CalculateTiltMeterValue(r, b);

            #endregion

            r.DidDelayedTurnCBet = a.DidDelayedTurnCBet + b.DidDelayedTurnCBet;
            r.CouldDelayedTurnCBet = a.CouldDelayedTurnCBet + b.CouldDelayedTurnCBet;
            r.DidDelayedTurnCBetIn3BetPot = a.DidDelayedTurnCBetIn3BetPot + b.DidDelayedTurnCBetIn3BetPot;
            r.CouldDelayedTurnCBetIn3BetPot = a.CouldDelayedTurnCBetIn3BetPot + b.CouldDelayedTurnCBetIn3BetPot;

            r.DidDonkBet = a.DidDonkBet + b.DidDonkBet;
            r.CouldDonkBet = a.CouldDonkBet + b.CouldDonkBet;
            r.FacedDonkBet = a.FacedDonkBet + b.FacedDonkBet;
            r.FoldedToDonkBet = a.FoldedToDonkBet + b.FoldedToDonkBet;

            r.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP = a.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP + b.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP;
            r.CheckedCalledTurnWhenCheckedFlopAsPfr = a.CheckedCalledTurnWhenCheckedFlopAsPfr + b.CheckedCalledTurnWhenCheckedFlopAsPfr;
            r.CheckedFoldedToTurnWhenCheckedFlopAsPfr = a.CheckedFoldedToTurnWhenCheckedFlopAsPfr + b.CheckedFoldedToTurnWhenCheckedFlopAsPfr;
            r.FacedTurnBetWhenCheckedFlopAsPfr = a.FacedTurnBetWhenCheckedFlopAsPfr + b.FacedTurnBetWhenCheckedFlopAsPfr;
            r.CalledTurnBetWhenCheckedFlopAsPfr = a.CalledTurnBetWhenCheckedFlopAsPfr + b.CalledTurnBetWhenCheckedFlopAsPfr;
            r.FoldedToTurnBetWhenCheckedFlopAsPfr = a.FoldedToTurnBetWhenCheckedFlopAsPfr + b.FoldedToTurnBetWhenCheckedFlopAsPfr;
            r.RaisedTurnBetWhenCheckedFlopAsPfr = a.RaisedTurnBetWhenCheckedFlopAsPfr + b.RaisedTurnBetWhenCheckedFlopAsPfr;

            r.FlopBetToPotRatio = b.FlopBetToPotRatio;
            r.TurnBetToPotRatio = b.TurnBetToPotRatio;
            r.RiverBetToPotRatio = b.RiverBetToPotRatio;

            r.DidFlopCheckBehind = a.DidFlopCheckBehind + b.DidFlopCheckBehind;
            r.CouldFlopCheckBehind = a.CouldFlopCheckBehind + b.CouldFlopCheckBehind;

            r.FoldedTurn = a.FoldedTurn + b.FoldedTurn;
            r.FacedBetOnTurn = a.FacedBetOnTurn + b.FacedBetOnTurn;

            r.CheckedCalledRiver = a.CheckedCalledRiver + b.CheckedCalledRiver;
            r.CheckedFoldedRiver = a.CheckedFoldedRiver + b.CheckedFoldedRiver;
            r.CheckedThenFacedBetOnRiver = a.CheckedThenFacedBetOnRiver + b.CheckedThenFacedBetOnRiver;

            r.RiverWonOnFacingBet = a.RiverWonOnFacingBet + b.RiverWonOnFacingBet;
            r.RiverCallSizeOnFacingBet = a.RiverCallSizeOnFacingBet + b.RiverCallSizeOnFacingBet;

            r.ShovedFlopAfter4Bet = a.ShovedFlopAfter4Bet + b.ShovedFlopAfter4Bet;
            r.CouldShoveFlopAfter4Bet = a.CouldShoveFlopAfter4Bet + b.CouldShoveFlopAfter4Bet;

            r.BetFlopWhenCheckedToSRP = a.BetFlopWhenCheckedToSRP + b.BetFlopWhenCheckedToSRP;
            r.CouldBetFlopWhenCheckedToSRP = a.CouldBetFlopWhenCheckedToSRP + b.CouldBetFlopWhenCheckedToSRP;

            r.FacedBetOnRiver = a.FacedBetOnRiver + b.FacedBetOnRiver;
            r.RiverVsBetFold = a.RiverVsBetFold + b.RiverVsBetFold;
            r.FlopCBetSuccess = a.FlopCBetSuccess + b.FlopCBetSuccess;
            r.DidCheckTurn = a.DidCheckTurn + b.DidCheckTurn;
            r.TotalCallAmountOnRiver = a.TotalCallAmountOnRiver + b.TotalCallAmountOnRiver;
            r.TotalWonAmountOnRiverCall = a.TotalWonAmountOnRiverCall + b.TotalWonAmountOnRiverCall;

            r.FirstRaiserPosition = b.FirstRaiserPosition;
            r.ThreeBettorPosition = b.ThreeBettorPosition;
            r.CouldProbeBetTurn = a.CouldProbeBetTurn + b.CouldProbeBetTurn;
            r.CouldProbeBetRiver = a.CouldProbeBetRiver + b.CouldProbeBetRiver;

            r.PreflopRaisersCount = b.PreflopRaisersCount;
            r.OpenRaisePreflopInBBs = b.OpenRaisePreflopInBBs;

            r.FacedDelayedCBet = a.FacedDelayedCBet + b.FacedDelayedCBet;

            return r;
        }

        /// <summary>
        /// Creates a shallow copy of the current object
        /// </summary>
        /// <returns>Copy of the current object</returns>
        public virtual Playerstatistic Copy()
        {
            return (Playerstatistic)MemberwiseClone();
        }

        /// <summary>
        /// Calculates total pot
        /// </summary>
        public virtual void CalculateTotalPot()
        {
            TotalPot = Pot;
            TotalPotInBB = (TotalPot != 0) && (BigBlind != 0) ? TotalPot / BigBlind : 0;
        }

        private static void CalculateTiltMeterValue(Playerstatistic source, Playerstatistic statistic)
        {
            // rule 1 - Calculate for every showdown the hero goes to and loses count as a 1. 
            if (statistic.Sawshowdown > 0 && statistic.Wonhand < 1)
            {
                source.TiltMeterPermanent++;
            }

            // rule 2 - calculate tilt meter - take only 14 hands, so using temp variable
            var tiltMeterValue = 0;

            // rule 2 - For every hand where hero has seen the flop, but folded at some point, count as a 1.
            if (statistic.Sawflop > 0 && statistic.PlayerFolded)
            {
                tiltMeterValue++;
            }

            // rule 2 - For every 50-100 BB pot that hero loses count a 1. For every 100+ BB pot hero loses count as 1 (so if pot lost was 110 bbs, point total would be 2)
            var potInBB = source.BigBlind != 0 ? source.Pot / source.BigBlind : 0;

            if (potInBB >= 50 && potInBB <= 100)
            {
                tiltMeterValue++;
            }
            else if (potInBB > 100)
            {
                tiltMeterValue += 2;
            }

            // reset 3-bet in row if player hasn't done 3-bet in new hand 
            if (statistic.Didthreebet == 0)
            {
                source.DidThreeBetInRow = 0;
            }

            source.DidThreeBetInRow += statistic.Didthreebet;

            // rule 2 - If hero 3-bets 4+ times in a row, count as a 2
            if (source.DidThreeBetInRow > 3)
            {
                tiltMeterValue += 2;
            }

            // reset pfr in row if new hand 
            if (statistic.Pfrhands == 0)
            {
                source.PFRInRow = 0;
            }

            source.PFRInRow += statistic.Pfrhands;

            // rule 2 - If hero PFR’s 5+ times in a row, count as a 2
            if (source.PFRInRow > 4)
            {
                tiltMeterValue += 2;
            }

            if (source.TiltMeterTemporaryHistory.Count > 17)
            {
                source.TiltMeterTemporaryHistory.Dequeue();
            }

            source.TiltMeterTemporaryHistory.Enqueue(tiltMeterValue);
        }

#pragma warning disable 0067
        // required to avoid binding leaks
        public virtual event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }
}
