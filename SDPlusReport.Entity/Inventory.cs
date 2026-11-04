using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDPlusReport.Entity
{
    public class Inventory
    {
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? VCollabId { get; set; }
        public  string? CreatedDate { get; set; }
        public string? AccountActivity { get; set; }
        public string? LastWorkLog { get; set; }
    }
}