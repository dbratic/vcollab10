using System.Reflection.Metadata.Ecma335;

namespace SDPlusReport.Entity
{
    public class HoursByAccount
    {        
        public string? AccountName { get; set; }  = string.Empty;      
        public string? Category { get; set; } = string.Empty;
        public string? Activity { get; set; } = string.Empty;
        public decimal January { get; set; } = 0.0m;
        public decimal February { get; set; } = 0.0m;
        public decimal March { get; set; } = 0.0m;
        public decimal April { get; set; } = 0.0m;
        public decimal May { get; set; } = 0.0m;
        public decimal June { get; set; } = 0.0m;
        public decimal July { get; set; } = 0.0m;
        public decimal August { get; set; }= 0.0m;
        public decimal September { get; set; } = 0.0m;
        public decimal October { get; set; } = 0.0m;
        public decimal November { get; set; } = 0.0m;
        public decimal December { get; set; } = 0.0m;
        public decimal YearTotal { get; set; } = 0.0m;

    }
}
