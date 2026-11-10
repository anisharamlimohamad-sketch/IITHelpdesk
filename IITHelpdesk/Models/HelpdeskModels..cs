namespace ItHelpdesk.Models
{
    public class IncidentReportViewModel
    {
        public string Id { get; set; } = "INC-" + new Random().Next(1000, 9999);
        public string EmployeeId { get; set; }
        public string AssetName { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Area { get; set; }
        public string Category { get; set; } = "General";
        public bool IsUrgent { get; set; } = false; 
        public string ProblemDescription { get; set; }
        public string Status { get; set; } = "New";
        public string AdminComments { get; set; } = "";
        public string UserReply { get; set; }
        public bool HasUnreadMessages { get; set; } = true;

        public List<IncidentActionLog> ActionLogs { get; set; } = new List<IncidentActionLog>();
    } 

    public class MisRequestViewModel
    {
        public string Id { get; set; } = "MIS-" + new Random().Next(100, 999);
        public string RequesterEmployeeId { get; set; }
        public string Department { get; set; }
        public string RequestType { get; set; }
        public string NewHireName { get; set; }
        public DateTime NeededDate { get; set; }
        public string Status { get; set; } = "New";
    }

    public class IncidentActionLog
    {
        public int Id { get; set; }
        public string IncidentId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty; // e.g., "Diagnosis Run", "Hardware Replaced"
        public string Notes { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
} 