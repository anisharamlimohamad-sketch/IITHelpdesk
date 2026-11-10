using ItHelpdesk.Models;
using System;
using System.Collections.Generic;

namespace ITHelpdesk.Models
{
    public static class DataStore
    {
        public static List<IncidentReportViewModel> Incidents = new()
        {
            new IncidentReportViewModel
            {
                Id = "1",
                EmployeeId = "TW00446",
                AssetName = "NGKLT186",
                Department = "Admin",
                Section = "MIS",
                Area = "Common Office Level 2",
                Category = "Laptop",
                ProblemDescription = "Battery problem",
                Status = "New",
                AdminComments = ""

            }
        };

        public static List<MisRequestViewModel> MisRequests = new()
        {
            new MisRequestViewModel
            {
                Id = "1",
                RequesterEmployeeId = "TW00446",
                Department = "Admin",
                RequestType = "New Laptop",
                NewHireName = "Anisha",
                NeededDate = DateTime.Now.AddDays(5),
                Status = "Pending"
            }
        };
    }
}