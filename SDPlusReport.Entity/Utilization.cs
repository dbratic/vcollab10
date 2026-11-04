using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class Utilization
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? DepartmentName { get; set; }
        public string? Location { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int BusinessDays { get; set; }
        public decimal BillableHours { get; set; }
        public decimal NonBillableHours { get; set; }
        public string? UtilizationPercentage { get; set; }
    }
}
