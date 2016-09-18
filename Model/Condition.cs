namespace Model
{
    /// <summary>
    /// Conditiopn entity for some poker situations
    /// </summary>
    public class Condition
    {
        public bool Possible { get; set; }
        public bool Made { get; set; }
        public bool Happened { get; set; }
        public bool Passed
        {
            get { return Possible && !Made; }
        }
    }
}