namespace DriveHUD.Importers.Builders.iPoker
{
    public enum ActionType : int
    {
        Fold = 0,
        SB = 1,
        BB = 2,
        Call = 3,
        Check = 4,
        Bet = 5,
        Raise = 6,
        AllIn = 7,    
        Ante = 15,
        RaiseTo = 23,
        Undefined = 100
    }
}