------------------------------
-- ###Hand Notes###
------------------------------
CREATE TABLE "HandNotes"
(
  HandNoteId bigserial NOT NULL,
  HandTag integer,  
  Note text,  
  HandNumber bigint NOT NULL,
  PokerSiteId smallint NOT NULL,
  CONSTRAINT HandNotes_PK PRIMARY KEY (HandNoteId)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "HandNotes"
  OWNER TO postgres;

------------------------------
-- ###Tournaments###
------------------------------
CREATE TABLE "Tournaments"
(
  TournamentId serial NOT NULL,
  TournamentNumber character varying(20) NOT NULL,
  PokerSiteId smallint NOT NULL,
  Tag text NOT NULL,
  BuyIn integer NOT NULL,
  Rake integer NOT NULL,
  Currency smallint NOT NULL,
  Filename text NOT NULL,
  FileLastModifiedTime timestamp without time zone NOT NULL,
  ImportType smallint NOT NULL,
  GameType smallint NOT NULL,
  SpeedType smallint NOT NULL,
  TablesQty smallint NOT NULL,
  TournamentSize integer NOT NULL,
  TableSize smallint NOT NULL,
  StartingStacks smallint NOT NULL,
  PlayerId integer NOT NULL,
  FirstHandTimestamp timestamp without time zone,
  LastHandTimestamp timestamp without time zone,
  PlayerFinished boolean NOT NULL,
  PlayerEndPosition integer NOT NULL,
  Winnings integer NOT NULL,  
  Rebuy integer NOT NULL,  
  Bounty integer DEFAULT 0,
  CONSTRAINT Tournaments_PK PRIMARY KEY (TournamentId)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "Tournaments"
  OWNER TO postgres;

CREATE INDEX TournamentPlayerIdTournamentNumber_IDX
  ON "Tournaments"
  USING btree
  (PlayerID, TournamentNumber);

CREATE INDEX TournamentTournamentNumberPokerSiteId_IDX
  ON "Tournaments"
  USING btree
  (TournamentNumber, PokerSiteId);
  
------------------------------
-- ###Game information###
------------------------------
CREATE TABLE GameInfo
(
  GameInfoId serial NOT NULL,
  BigBlind integer NOT NULL,
  SmallBlind integer NOT NULL,
  Ante integer NOT NULL,
  IsTournament boolean NOT NULL,
  Currency smallint NOT NULL,
  GameType smallint NOT NULL,
  TableSize smallint NOT NULL,
  CONSTRAINT GameInfo_PK PRIMARY KEY (GameInfoId)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE GameInfo
  OWNER TO postgres;
 
------------------------------
-- ###Hand Histories###
------------------------------
CREATE TABLE HandHistories
(
  HandHistoryId bigserial NOT NULL,
  HandNumber bigint NOT NULL,
  PokerSiteId smallint NOT NULL,
  HandHistory text NOT NULL,  
  HandHistoryTimestamp timestamp without time zone,
  GameType integer,
  TournamentNumber character varying(20),
  CONSTRAINT HandHistories_PK PRIMARY KEY (HandHistoryId),
  CONSTRAINT HandHistories_HandNumber_UK UNIQUE (HandNumber)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE HandHistories
  OWNER TO postgres;
 
CREATE INDEX HandHistoriesHandNumberPokerSiteId_IDX
  ON handhistories
  USING btree
  (HandNumber, PokerSiteId);

CREATE INDEX HandHistoriesHandHistoryTimestampGameType_IDX
  ON handhistories
  USING btree
  (HandHistoryTimestamp, GameType);

------------------------------
-- ###Players###
------------------------------
CREATE TABLE "Players"
(
  "PlayerId" serial NOT NULL,
  "PlayerName" text NOT NULL,
  "PokerSiteId" smallint NOT NULL,
  "CashHandsPlayed" integer NOT NULL,
  "TournamentHandsPlayed" integer NOT NULL,  
  CONSTRAINT Players_PK PRIMARY KEY (PlayerId)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE Players
  OWNER TO postgres;
 
CREATE INDEX PlayersPlayerNamePokerSiteId_IDX
  ON Players
  USING btree
  (PlayerName, PokerSiteId);

------------------------------
-- ###Hand Records###
------------------------------
CREATE TABLE HandRecords
(
  HandRecordId serial NOT NULL,
  PlayerId integer NOT NULL,
  HandRecordTimestamp timestamp without time zone NOT NULL,
  GameInfoId int NOT NULL,
  Cards text,
  Line text,
  Board text,
  NetWon integer NOT NULL,
  BBWon integer NOT NULL,
  Position text,
  Action text,
  AllIn text,
  Equity numeric,
  EquityDiff numeric,
  CONSTRAINT HandRecords_PK PRIMARY KEY (HandRecordId),
  CONSTRAINT HandRecords_GameInfoId_FK FOREIGN KEY (GameInfoId)
      REFERENCES GameInfo (GameInfoId) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION,
  CONSTRAINT HandRecords_PlayerId_FK FOREIGN KEY (PlayerId)
      REFERENCES Players (PlayerId) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE HandRecords
  OWNER TO postgres;

------------------------------
-- ###PlayerStatistic###
------------------------------
CREATE TABLE PlayerStatistic
(
  PlayerStatisticId bigserial NOT NULL,
  PlayerId integer NOT NULL,
  PlayedDate integer NOT NULL, 
  TotalPlayers smallint NOT NULL, 
  GameInfoId smallint NOT NULL, 
  TotalHands integer NOT NULL, 
  TotalWon integer NOT NULL, 
  TotalRake integer NOT NULL, 
  TotalWonBB integer NOT NULL,    
  BigBlindStealAttempted integer NOT NULL,
  BigBlindStealDefended integer NOT NULL,
  BigBlindStealReraised integer NOT NULL,
  CalledFlopContinuationBet integer NOT NULL,
  CalledFourBetPreflop integer NOT NULL,
  CalledRiverContinuationBet integer NOT NULL,
  CalledThreeBetPreflop integer NOT NULL,
  CalledTurnContinuationBet integer NOT NULL,
  CalledTwoPreflopRaisers integer NOT NULL,
  CouldColdCall integer NOT NULL,  
  CouldSqueeze integer NOT NULL,
  CouldThreeBet integer NOT NULL,
  DidColdCall integer NOT NULL,
  DidSqueeze integer NOT NULL,
  DidThreeBet integer NOT NULL,
  FacedFourBetPreflop integer NOT NULL,
  FacedThreeBetPreflop integer NOT NULL,
  FacingFlopContinuationBet integer NOT NULL,
  FacingRiverContinuationBet integer NOT NULL,
  FacingTurnContinuationBet integer NOT NULL,
  FacingTwoPreFlopRaisers integer NOT NULL,
  FlopContinuationBetMade integer NOT NULL,
  FlopContinuationBetPossible integer NOT NULL,
  FoldedToFlopContinuationBet integer NOT NULL,
  FoldedToFourBetPreflop integer NOT NULL,
  FoldedToRiverContinuationBet integer NOT NULL,
  FoldedToThreeBetPreflop integer NOT NULL,
  FoldedToTurnContinuationBet integer NOT NULL,
  Pfr integer NOT NULL, 
  RaisedFlopContinuationBet integer NOT NULL,
  RaisedFourBetPreflop integer NOT NULL,
  RaisedRiverContinuationBet integer NOT NULL,
  RaisedThreeBetPreflop integer NOT NULL,
  RaisedTurnContinuationBet integer NOT NULL,
  RaisedTwoPreflopRaisers integer NOT NULL,
  RiverCallIpPassOnTurnCb integer NOT NULL,
  RiverContinuationBetMade integer NOT NULL,
  RiverContinuationBetPossible integer NOT NULL,
  RiverFoldIpPassOnTurnCb integer NOT NULL,
  RiverRaiseIpPassOnTurnCb integer NOT NULL,
  SawFlop integer NOT NULL,
  SawLargeShowdown integer NOT NULL,
  SawLargeShowdownLimpedFlop integer NOT NULL,
  SawNonSmallShowdown integer NOT NULL,
  SawNonSmallShowdownLimpedFlop integer NOT NULL,
  SawShowdown integer NOT NULL,
  SmallBlindStealAttempted integer NOT NULL,
  SmallBlindStealDefended integer NOT NULL,
  SmallBlindStealReraised integer NOT NULL,
  TotalAggressivePostflopStreetsSeen integer NOT NULL,
  TotalBets integer NOT NULL,
  TotalCalls integer NOT NULL,
  TotalPostFlopStreetsSeen integer NOT NULL,
  TurnCallIpPassOnFlopCb integer NOT NULL,
  TurnContinuationBetMade integer NOT NULL,
  TurnContinuationBetPossible integer NOT NULL,
  TurnFoldIpPassOnFlopCb integer NOT NULL,
  TurnRaiseIpPassOnFlopCb integer NOT NULL,
  Vpip integer NOT NULL, 
  WonHand integer NOT NULL,
  WonHandWhenSawFlop integer NOT NULL,
  WonHandWhenSawRiver integer NOT NULL,
  WonHandWhenSawTurn integer NOT NULL,
  WonLargeShowdown integer NOT NULL,
  WonLargeShowdownLimpedFlop integer NOT NULL,
  WonNonSmallShowdown integer NOT NULL,
  WonNonSmallShowdownLimpedFlop integer NOT NULL,
  WonShowdown integer NOT NULL,
  CONSTRAINT PlayerStatistic_PK PRIMARY KEY (PlayerStatisticId)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE PlayerStatistic
  OWNER TO postgres;

CREATE INDEX PlayerStatisticPlayerIdPlayedDateTotalPlayersGameInfoId_IDX
  ON playerstatistic
  USING btree
  (PlayerId, PlayedDate, TotalPlayers, GameInfoId);  