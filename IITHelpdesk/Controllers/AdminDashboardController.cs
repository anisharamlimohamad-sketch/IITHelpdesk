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
            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            return View("~/Views/Helpdesk/TicketDetails.cshtml", incident);
        }
        [HttpGet]
        public IActionResult GetChatMessages(string id)
        {
            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            // Menjana semula paparan HTML mesej chat secara langsung untuk dihantar ke JavaScript (AJAX)
            string htmlContent = "";

            if (string.IsNullOrWhiteSpace(incident.UserReply))
            {
                htmlContent = @"<div style=""flex: 1; display: grid; place-items: center; text-align: center; color: var(--muted);"">
                                <div>
                                    <i class=""fa-regular fa-comment-dots fs-3 mb-2 text-primary""></i>
                                    <div style=""font-weight: 600; color: var(--text);"">No messages yet</div>
                                    <div style=""font-size: 12px;"">Say hello to start the conversation.</div>
                                </div>
                              </div>";
            }
            else
            {
                htmlContent += @"<div style=""text-align: center; margin: 8px 0 16px 0;"">
                                    <span style=""background: #fff; padding: 4px 12px; border-radius: 8px; font-size: 11px; color: #54656f; box-shadow: 0 1px 1px rgba(0,0,0,0.05);"">Today</span>
                                  </div>";

                var messages = incident.UserReply.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var msg in messages)
                {
                    bool isAdmin = msg.StartsWith("[Admin");
                    string cleanMsg = msg;
                    string timeOnly = DateTime.Now.ToString("HH:mm");

                    int bracketIndex = msg.IndexOf("] ");
                    if (bracketIndex != -1 && msg.StartsWith("["))
                    {
                        string headerPart = msg.Substring(1, bracketIndex - 1);
                        if (headerPart.Contains(" "))
                        {
                            var parts = headerPart.Split(' ');
                            if (parts.Length > 1) { timeOnly = parts[parts.Length - 1]; }
                        }
                        cleanMsg = msg.Substring(bracketIndex + 2);
                    }

                    string bubbleClass = isAdmin ? "chat-bubble-out" : "chat-bubble-in";

                    htmlContent += $@"<div class=""{bubbleClass}"">
                                        <span>{cleanMsg}</span>
                                        <span class=""chat-time"">
                                            {timeOnly}";

                    if (isAdmin)
                    {
                        htmlContent += @"<i class=""fa-solid fa-check-double"" style=""color: #53bdeb; font-size: 9px; margin-left: 2px;""></i>";
                    }

                    htmlContent += "</span></div>";
                }
            }

            return Content(htmlContent, "text/html");
        }
    }

    public class AdminDashboardTuple
    {
        public List<IncidentReportViewModel> Incidents { get; set; }
        public List<MisRequestViewModel> MisRequests { get; set; }
        public List<IncidentActionLog> ActionLogs { get; set; } = new List<IncidentActionLog>();
    }
}
