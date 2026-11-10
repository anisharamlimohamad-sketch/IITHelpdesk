using ItHelpdesk.Models;
using ITHelpdesk.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItHelpdesk.Controllers
{
    public class TrackStatusController : Controller
    {
        public IActionResult TrackStatus(string searchQuery)
        {
            ViewData["Query"] = searchQuery;
            var results = string.IsNullOrEmpty(searchQuery)
                ? new List<IncidentReportViewModel>()
                : DataStore.Incidents.Where(i =>
                    (!string.IsNullOrEmpty(i.EmployeeId) && i.EmployeeId.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.AssetName) && i.AssetName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Department) && i.Department.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Section) && i.Section.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(i.Area) && i.Area.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                  ).ToList();

            // Guna path Views/Helpdesk
            return View("~/Views/Helpdesk/TrackStatus.cshtml", results);
        }

        [HttpPost]
        public IActionResult SubmitUserReply(string id, string userReply)
        {
            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == id);

            if (incident != null && !string.IsNullOrWhiteSpace(userReply))
            {
                string timeTag = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                if (string.IsNullOrWhiteSpace(incident.UserReply) && !string.IsNullOrWhiteSpace(incident.AdminComments))
                {
                    incident.UserReply = $"[Admin - {timeTag}] {incident.AdminComments}";
                }

                string userMsg = $"[You - {timeTag}] {userReply}";

                if (string.IsNullOrWhiteSpace(incident.UserReply))
                {
                    incident.UserReply = userMsg;
                }
                else
                {
                    incident.UserReply = incident.UserReply + "\n" + userMsg;
                }
            }

            return RedirectToAction("TrackStatus", new { searchQuery = incident?.EmployeeId });
        }
    }
}