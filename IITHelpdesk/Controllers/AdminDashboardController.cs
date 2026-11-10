using ItHelpdesk.Models;
using ITHelpdesk.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItHelpdesk.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult AdminDashboard()
        {
            ViewBag.IncidentTotal = DataStore.Incidents.Count;
            ViewBag.IncidentNew = DataStore.Incidents.Count(i => i.Status == "New");
            ViewBag.IncidentPending = DataStore.Incidents.Count(i => i.Status == "Pending");
            ViewBag.IncidentAssigned = DataStore.Incidents.Count(i => i.Status == "Assigned");
            ViewBag.IncidentInvestigating = DataStore.Incidents.Count(i => i.Status == "Investigating");
            ViewBag.IncidentResolved = DataStore.Incidents.Count(i => i.Status == "Resolved");
            ViewBag.IncidentClosed = DataStore.Incidents.Count(i => i.Status == "Closed");

            ViewBag.MisTotal = DataStore.MisRequests.Count;
            ViewBag.MisPending = DataStore.MisRequests.Count(m => m.Status == "Pending");
            ViewBag.MisInProgress = DataStore.MisRequests.Count(m => m.Status == "In Progress");
            ViewBag.MisApproved = DataStore.MisRequests.Count(m => m.Status == "Approved");

            var model = new AdminDashboardTuple
            {
                Incidents = DataStore.Incidents,
                MisRequests = DataStore.MisRequests,
                // Jika DataStore simpan action logs secara global, boleh bind di sini:
                // ActionLogs = DataStore.ActionLogs 
            };

            // Guna path Views/Helpdesk
            return View("~/Views/Helpdesk/AdminDashboard.cshtml", model);
        }

        [HttpPost]
        public IActionResult UpdateIncidentStatus(string id, string status, string comments)
        {
            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == id);
            if (incident != null)
            {
                if (!string.IsNullOrWhiteSpace(comments))
                {
                    string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    string formattedComment = $"[Admin - {timeStamp}] {comments}";

                    if (string.IsNullOrWhiteSpace(incident.UserReply))
                    {
                        incident.UserReply = formattedComment;
                    }
                    else
                    {
                        incident.UserReply += "\r\n" + formattedComment;
                    }
                    incident.AdminComments = comments;
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    incident.Status = status;
                }
            }

            // Jika dipanggil melalui fetch/AJAX, pulangkan status OK (200)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("json"))
            {
                return Ok();
            }

            return RedirectToAction("TicketDetails", new { id = id });
        }

        [HttpPost]
        public IActionResult UpdateMisStatus(string id, string status)
        {
            var mis = DataStore.MisRequests.FirstOrDefault(m => m.Id == id);
            if (mis != null)
            {
                mis.Status = status;
            }
            return RedirectToAction("AdminDashboard");
        }

        // Method AddActionLog dipindahkan KE DALAM Controller dengan betul
        [HttpPost]
        public IActionResult AddActionLog(string incidentId, string staffName, string notes)
        {
            var newLog = new IncidentActionLog
            {
                IncidentId = incidentId, // Keep as string
                StaffName = staffName,
                ActionType = "General Action",
                Notes = notes,
                Timestamp = DateTime.Now
            };

            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == incidentId);
            if (incident != null)
            {
                incident.ActionLogs.Add(newLog);
            }

            return RedirectToAction("TicketDetails", new { id = incidentId });
        }

        [HttpGet]
        public IActionResult TicketDetails(string id)
        {
            // Cari incident berdasarkan ID daripada DataStore
            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            // Hantar model incident ke view baharu
            return View("~/Views/Helpdesk/TicketDetails.cshtml", incident);
        }
    }

    public class AdminDashboardTuple
    {
        public List<IncidentReportViewModel> Incidents { get; set; }
        public List<MisRequestViewModel> MisRequests { get; set; }
        public List<IncidentActionLog> ActionLogs { get; set; } = new List<IncidentActionLog>();
    }
}
