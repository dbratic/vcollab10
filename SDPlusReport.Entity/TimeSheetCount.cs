using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class TimeSheetCount
    {
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? EmployeeType { get; set; }
        public string? EmpLocation { get; set; }
        public string? Approver { get; set; }
        public string? CreateDate { get; set; }
        public string? TimesheetStartDate { get; set; }
        public string? TimesheetEndDate { get; set; }
        public string? LastTimesheetDate { get; set; }
        public int EntryCount { get; set; }
    }
}
