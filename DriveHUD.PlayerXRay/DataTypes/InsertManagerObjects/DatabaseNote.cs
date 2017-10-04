namespace DriveHUD.PlayerXRay.DataTypes.InsertManagerObjects
{
    public class DatabaseNote
    {
        public bool Added { get; set; }
        public bool Modified { get; set; }

        public long PlayerID { get; set; }
        public string Message { get; set; }
    }
}