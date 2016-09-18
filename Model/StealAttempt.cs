namespace Model
{
    /// <summary>
    /// Entity that discribes player reaction to steal attempt
    /// </summary>
    public class StealAttempt
    {
        public bool Attempted { get; set; }

        public bool Defended { get; set; }

        public bool Raised { get; set; }

        public bool Folded { get; set; }
    }
}