using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class Report
    {
        public int WorkLogId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? EmploymentType { get; set; }
        public string? DepartmentName { get; set; }
        public string? Location { get; set; }
        public bool IsBillable { get; set; }
        public string? BillingStatus { get; set; }
        public string? RecordStartDate { get; set; }
        public string? RecordEndDate { get; set; }
        public string? WorkLogCreatedDate { get; set; }
        public string? WorkLogCreatedTime { get; set; }
        public decimal TimeSpentHrs { get; set; }
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? VcOldProjectId { get; set; }
        public string? Module { get; set; }
        public string? ModuleId { get; set; }
        public string? ModuleTitle { get; set; }
        public string? RequestType { get; set; }
        public string? TaskId { get; set; }
        public string? TaskTitle { get; set; }
        public string? WorkLogDescription { get; set; }
        public string? CreatedBy { get; set; }

    }
}


