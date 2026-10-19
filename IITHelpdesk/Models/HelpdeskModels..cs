namespace ItHelpdesk.Models
{
    public class IncidentReportViewModel
    {
        public string Id { get; set; } = "INC-" + new Random().Next(1000, 9999);
        public string EmployeeId { get; set; }
        public string AssetName { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string ProblemDescription { get; set; }
        public string Status { get; set; } = "Pending";

        public string AdminComments { get; set; } = "";
    }

    public class MisRequestViewModel
    {
        public string Id { get; set; } = "MIS-" + new Random().Next(100, 999);
        public string RequesterEmployeeId { get; set; }
        public string Department { get; set; }
        public string RequestType { get; set; }
        public string NewHireName { get; set; }
        public DateTime NeededDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}