using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class TimeSheet
    {
        public int TimeSheetId { get; set; }
        public int TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public string? Department { get; set; }
        public string? TimesheetStartDate { get; set; }
        public string? TimesheetEndDate { get; set; }
        public double TotalTimeSpent { get; set; }
        public string? TimeSheetStatus { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovedDate { get; set; }
        public string? EmpLocation { get; set; }
        public string? EmploymentType { get; set; }

    }
}
