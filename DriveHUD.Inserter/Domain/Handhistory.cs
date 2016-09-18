using System;

namespace DriveHUD.Inserter.Domain {
    
    internal partial class Handhistory 
    {
        public virtual int HandhistoryId { get; set; }
        public virtual long Gamenumber { get; set; }
        public virtual string Tourneynumber { get; set; }
        public virtual string HandhistoryVal { get; set; }
        public virtual int? GametypeId { get; set; }
        public virtual short PokersiteId { get; set; }
        public virtual DateTime? Handtimestamp { get; set; }
        public virtual string Filename { get; set; }
    }
}
