using System;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;
using AcePokerSolutions.DataAccessHelper.DriveHUD;
using DriveHUD.PlayerXRay.DataTypes;

namespace AcePokerSolutions.DataAccessHelper
{
    [ProtoContract]
    public class Playerstatistic
    {
        public override string ToString()
        {
            return $"Player: {PlayerName}; Tournament: {TournamentId}; Time: {Time}; HandNumber: {GameNumber}; Currency: {CurrencyId}; Cards: {Cards}";
        }

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

        [Required, ProtoMember(24)]
        public virtual short Numberofplayers { get; set; }

        [Required, ProtoMember(289)]
        public virtual short MaxNumberofplayers { get; set; }

        [Required, ProtoMember(290)]
        public virtual HandHistory HandHistory { get; set; }

        public static Playerstatistic operator +(Playerstatistic a, Playerstatistic b)
        {
            Playerstatistic r = new Playerstatistic();
            r.GameNumber = b.GameNumber;
            r.PokersiteId = b.PokersiteId;
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

            r.DidThreeBetInBb += a.DidThreeBetInBb + b.DidThreeBetInBb;
            r.DidThreeBetInBtn += a.DidThreeBetInBtn + b.DidThreeBetInBtn;
            r.DidThreeBetInCo += a.DidThreeBetInCo + b.DidThreeBetInCo;
            r.DidThreeBetInMp += a.DidThreeBetInMp + b.DidThreeBetInMp;
            r.DidThreeBetInSb += a.DidThreeBetInSb + b.DidThreeBetInSb;

            r.DidthreebetBluffInSb = a.DidthreebetBluffInSb + b.DidthreebetBluffInSb;
            r.DidthreebetBluffInBb = a.DidthreebetBluffInBb + b.DidthreebetBluffInBb;
            r.DidthreebetBluffInBlinds = a.DidthreebetBluffInBlinds + b.DidthreebetBluffInBlinds;

            r.DidFourBetInBb += a.DidFourBetInBb + b.DidFourBetInBb;
            r.DidFourBetInBtn += a.DidFourBetInBtn + b.DidFourBetInBtn;
            r.DidFourBetInCo += a.DidFourBetInCo + b.DidFourBetInCo;
            r.DidFourBetInMp += a.DidFourBetInMp + b.DidFourBetInMp;
            r.DidFourBetInSb += a.DidFourBetInSb + b.DidFourBetInSb;

            r.DidfourbetBluff = a.DidfourbetBluff + b.DidfourbetBluff;
            r.DidFourBetBluffInBtn = a.DidFourBetBluffInBtn + b.DidFourBetBluffInBtn;

            r.DidColdCallInBb += a.DidColdCallInBb + b.DidColdCallInBb;
            r.DidColdCallInBtn += a.DidColdCallInBtn + b.DidColdCallInBtn;
            r.DidColdCallInCo += a.DidColdCallInCo + b.DidColdCallInCo;
            r.DidColdCallInMp += a.DidColdCallInMp + b.DidColdCallInMp;
            r.DidColdCallInSb += a.DidColdCallInSb + b.DidColdCallInSb;
            r.DidColdCallInEp += a.DidColdCallInEp + b.DidColdCallInEp;
            r.DidColdCallThreeBet += a.DidColdCallThreeBet + b.DidColdCallThreeBet;
            r.CouldColdCallThreeBet += a.CouldColdCallThreeBet + b.CouldColdCallThreeBet;
            r.DidColdCallFourBet += a.DidColdCallFourBet + b.DidColdCallFourBet;
            r.CouldColdCallFourBet += a.CouldColdCallFourBet + b.CouldColdCallFourBet;
            r.DidColdCallVsOpenRaiseBtn += a.DidColdCallVsOpenRaiseBtn + b.DidColdCallVsOpenRaiseBtn;
            r.DidColdCallVsOpenRaiseCo += a.DidColdCallVsOpenRaiseCo + b.DidColdCallVsOpenRaiseCo;
            r.DidColdCallVsOpenRaiseSb += a.DidColdCallVsOpenRaiseSb + b.DidColdCallVsOpenRaiseSb;
            r.CouldColdCallVsOpenRaiseBtn += a.CouldColdCallVsOpenRaiseBtn + b.CouldColdCallVsOpenRaiseBtn;
            r.CouldColdCallVsOpenRaiseCo += a.CouldColdCallVsOpenRaiseCo + b.CouldColdCallVsOpenRaiseCo;
            r.CouldColdCallVsOpenRaiseSb += a.CouldColdCallVsOpenRaiseSb + b.CouldColdCallVsOpenRaiseSb;

            r.FirstRaiser = a.FirstRaiser + b.FirstRaiser;

            r.StealPossible = a.StealPossible + b.StealPossible;
            r.StealMade = a.StealMade + b.StealMade;

            r.TournamentId = b.TournamentId;



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
            r.CheckFoldFlop3BetOop = a.CheckFoldFlop3BetOop + b.CheckFoldFlopPfrOop;
            r.BetFoldFlopPfrRaiser = a.BetFoldFlopPfrRaiser + b.BetFoldFlopPfrRaiser;
            r.BetFlopCalled3BetPreflopIp = a.BetFlopCalled3BetPreflopIp + b.BetFlopCalled3BetPreflopIp;
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

            //r.PositionDidColdCall = PositionalStat.Sum(a.PositionDidColdCall, b.PositionDidColdCall);    
            //r.PositionCouldColdCall = PositionalStat.Sum(a.PositionCouldColdCall, b.PositionCouldColdCall);
            //r.PositionDidThreeBet = PositionalStat.Sum(a.PositionDidThreeBet, b.PositionDidThreeBet);
            //r.PositionCouldThreeBet = PositionalStat.Sum(a.PositionCouldThreeBet, b.PositionCouldThreeBet);
            //r.PositionDidFourBet = PositionalStat.Sum(a.PositionDidFourBet, b.PositionDidFourBet);
            //r.PositionCouldFourBet = PositionalStat.Sum(a.PositionCouldFourBet, b.PositionCouldFourBet);

            r.MRatio = b.MRatio;
            r.StackInBBs = b.StackInBBs;

            r.EVDiff = a.EVDiff + b.EVDiff;

            r.PreflopIP = a.PreflopIP + b.PreflopIP;
            r.PreflopOOP = a.PreflopOOP + b.PreflopOOP;       

            return r;
        }
    }
}
