using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class TimeSheetFilter
    {
        public string? Department { get; set; }
        public string? EmployeeType { get; set; }
        public string? Location { get; set; }
        public string? Approver { get; set; }
        public DateTime? TimesheetStartDate { get; set; }
        public DateTime? TimesheetEndDate { get; set; }
        public string? Status { get; set; }
    }
}
