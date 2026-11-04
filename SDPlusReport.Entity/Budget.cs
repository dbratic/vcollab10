namespace SDPlusReport.Entity
{
    public class Budget
    {
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }         
        public string? OpenDate { get; set; }
        //public DateTime? CloseDate { get; set; }
        public string? Status { get; set; }
        public string? TicketNumber { get; set; }
        public string? TicketSubject { get; set; }
        public string? TaskNumber { get; set; }
        public string? TaskName { get; set; }
        public double EstimatedHours { get; set; }
        public double ActualHours { get; set; }
        public double BillableHours { get; set; }
        public  double NonBillableHours { get; set; }
        public double HourDifference { get; set; }
    }
}
