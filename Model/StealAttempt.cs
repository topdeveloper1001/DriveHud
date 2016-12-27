namespace Model
{
    /// <summary>
    /// Entity that discribes player reaction to steal attempt or steal possibility
    /// </summary>
    public class StealAttempt
    {
        public bool Possible { get; set; }

        public bool Attempted { get; set; }

        public bool Defended { get; set; }

        public bool Raised { get; set; }

        public bool Folded { get; set; }

        public bool Faced { get; set; }
    }
}