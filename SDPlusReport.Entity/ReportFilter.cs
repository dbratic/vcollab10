using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class ReportFilter
    {
        public string? UserId { get; set; }
        public string? ClientId { get; set; }
        public string? AccountId { get; set; }
        public string? Module { get; set; }
        public string? RequestType { get; set; }
        public string? IsBillable { get; set; }
        public DateTime RecordStartDate { get; set; }
        public DateTime RecordEndDate { get; set; }
        public string? RecordDate { get; set; }
        public string? ReportName { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public string? ReportType { get; set; }
        public string? FilterType { get; set; }
    }
}
