using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class Compliance
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? EmploymentType { get; set; }
        public string? DepartmentName { get; set; }
        public string? Location { get; set; }
        public decimal TotalHours { get; set; }
    }
}
