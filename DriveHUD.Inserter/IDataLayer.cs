namespace DriveHUD.Inserter
{
    public interface IDataLayer
    {
        void Store(long gameNumber, string handHistory, int pokerClient = 1);
    }
}