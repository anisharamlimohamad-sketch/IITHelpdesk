using Microsoft.AspNetCore.Mvc;
using ItHelpdesk.Models;

namespace ItHelpdesk.Controllers
{
    public class HelpdeskController : Controller
    {
        // Sample static list for Incidents
        private static List<IncidentReportViewModel> _incidents = new()
        {
            new IncidentReportViewModel
            {
                EmployeeId = "EMP-402",
                AssetName = "LPT-DELL-04",
                Department = "Finance",
                Category = "Laptop",
                ProblemDescription = "Blue screen on boot",
                Status = "Pending",
                AdminComments = "" // Disetkan kosong secara automatik
            }
        };

        // Sample static list for MIS Requests
        private static List<MisRequestViewModel> _misRequests = new()
        {
            new MisRequestViewModel
            {
                RequesterEmployeeId = "EMP-102",
                Department = "Engineering",
                RequestType = "New Laptop",
                NewHireName = "John Doe",
                NeededDate = DateTime.Now.AddDays(5),
                Status = "Pending"
            }
        };

        public IActionResult UserPortal() => View();

        [HttpPost]
        public IActionResult SubmitIncident(IncidentReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Biarkan AdminComments kosong bila baru dihantar oleh user
                if (string.IsNullOrWhiteSpace(model.AdminComments))
                {
                    model.AdminComments = "";
                }

                _incidents.Add(model);
                TempData["SuccessMessage"] = $"Success! Incident report submitted for Employee ID: {model.EmployeeId} (Asset: {model.AssetName}).";
            }
            return RedirectToAction("UserPortal");
        }

        [HttpPost]
        public IActionResult SubmitMis(MisRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                _misRequests.Add(model);
                TempData["MisSuccess"] = $"Request logged successfully! Requester ID: {model.RequesterEmployeeId}.";
            }
            return RedirectToAction("UserPortal");
        }

        public IActionResult TrackStatus(string searchQuery)
        {
            ViewData["Query"] = searchQuery;
            var results = string.IsNullOrEmpty(searchQuery)
                ? new List<IncidentReportViewModel>()
                : _incidents.Where(i =>
                    (!string.IsNullOrEmpty(i.EmployeeId) && i.EmployeeId.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.AssetName) && i.AssetName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Department) && i.Department.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                  ).ToList();

            return View(results);
        }

        public IActionResult AdminDashboard()
        {
            ViewBag.TotalTickets = _incidents.Count + _misRequests.Count;
            ViewBag.PendingReports = _incidents.Count(i => i.Status == "Pending") + _misRequests.Count(m => m.Status == "Pending");
            ViewBag.InProgress = _incidents.Count(i => i.Status == "In Progress") + _misRequests.Count(m => m.Status == "In Progress");
            ViewBag.Closed = _incidents.Count(i => i.Status == "Resolved") + _misRequests.Count(m => m.Status == "Approved");

            var data = new AdminDashboardTuple { Incidents = _incidents, MisRequests = _misRequests };
            return View(data);
        }

        [HttpPost]
        public IActionResult UpdateIncidentStatus(string id, string status, string comments)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);
            if (incident != null)
            {
                incident.Status = status;
                incident.AdminComments = comments; // Menyimpan ulasan manual daripada pihak Admin/MIS
            }
            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public IActionResult UpdateMisStatus(string id, string status)
        {
            var mis = _misRequests.FirstOrDefault(m => m.Id == id);
            if (mis != null)
            {
                mis.Status = status;
            }
            return RedirectToAction("AdminDashboard");
        }
    }

    public class AdminDashboardTuple
    {
        public List<IncidentReportViewModel> Incidents { get; set; }
        public List<MisRequestViewModel> MisRequests { get; set; }
    }
}