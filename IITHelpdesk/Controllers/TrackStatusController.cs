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
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                // Ditandakan dengan [You] supaya sistem kenal ini mesej daripada user
                string formattedReply = $"[You - {timeStamp}] {userReply}";

                if (string.IsNullOrWhiteSpace(incident.UserReply))
                {
                    incident.UserReply = formattedReply;
                }
                else
                {
                    incident.UserReply += "\r\n" + formattedReply;
                }
            }

            // Jika dipanggil secara AJAX, balas dengan Ok() tanpa redirect page
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("json"))
            {
                return Ok();
            }

            // Fallback jika bukan AJAX
            return RedirectToAction("TrackStatus");
        }

        [HttpGet]
        public IActionResult GetTrackStatusChat(string id)
        {
            var item = DataStore.Incidents.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            string htmlContent = "";

            // Paparan asal Admin Comments (jika ada)
            if (!string.IsNullOrWhiteSpace(item.AdminComments) && string.IsNullOrWhiteSpace(item.UserReply))
            {
                htmlContent += $@"<div class=""d-flex align-items-start gap-2 mb-2"">
                            <div class=""bg-primary text-white rounded-circle d-flex align-items-center justify-content-center"" style=""width: 32px; height: 32px; min-width: 32px;"">
                                <i class=""fa-solid fa-headset"" style=""font-size: 0.85rem;""></i>
                            </div>
                            <div class=""bg-white p-3 rounded-3 shadow-sm border"" style=""max-width: 80%;"">
                                <strong class=""small text-primary d-block mb-1"">MIS Admin</strong>
                                <span class=""text-dark"" style=""font-size: 0.95rem;"">{item.AdminComments}</span>
                            </div>
                          </div>";
            }

            // Bubble Chat Loop
            if (!string.IsNullOrWhiteSpace(item.UserReply))
            {
                var messages = item.UserReply.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var msg in messages)
                {
                    bool isAdmin = msg.StartsWith("[Admin");
                    bool isUser = msg.StartsWith("[You");

                    string cleanMsg = msg;
                    string timeStr = "";
                    if (msg.Contains("]"))
                    {
                        var parts = msg.Split(new[] { ']' }, 2);
                        timeStr = parts[0].Trim('[');
                        cleanMsg = parts.Length > 1 ? parts[1].Trim() : msg;
                    }

                    if (isAdmin)
                    {
                        htmlContent += $@"<div class=""d-flex align-items-start gap-2 mb-2"">
                                    <div class=""bg-primary text-white rounded-circle d-flex align-items-center justify-content-center shadow-sm"" style=""width: 32px; height: 32px; min-width: 32px;"">
                                        <i class=""fa-solid fa-headset"" style=""font-size: 0.85rem;""></i>
                                    </div>
                                    <div class=""bg-white p-3 rounded-3 shadow-sm border"" style=""max-width: 80%;"">
                                        <div class=""d-flex justify-content-between align-items-center gap-3 mb-1"">
                                            <strong class=""small text-primary"">MIS Admin</strong>
                                            <span class=""text-muted"" style=""font-size: 0.7rem;"">{timeStr}</span>
                                        </div>
                                        <span class=""text-dark"" style=""font-size: 0.95rem; word-break: break-word;"">{cleanMsg}</span>
                                    </div>
                                  </div>";
                    }
                    else if (isUser)
                    {
                        htmlContent += $@"<div class=""d-flex align-items-start gap-2 justify-content-end mb-2"">
                                    <div class=""text-white p-3 rounded-3 shadow-sm"" style=""max-width: 80%; background: linear-gradient(135deg, #4f46e5 0%, #4338ca 100%);"">
                                        <div class=""d-flex justify-content-between align-items-center gap-3 mb-1"">
                                            <strong class=""small text-white-50"">You (Employee)</strong>
                                            <span class=""text-white-50"" style=""font-size: 0.7rem;"">{timeStr}</span>
                                        </div>
                                        <span class=""text-white"" style=""font-size: 0.95rem; word-break: break-word;"">{cleanMsg}</span>
                                    </div>
                                    <div class=""bg-secondary text-white rounded-circle d-flex align-items-center justify-content-center shadow-sm"" style=""width: 32px; height: 32px; min-width: 32px;"">
                                        <i class=""fa-solid fa-user"" style=""font-size: 0.85rem;""></i>
                                    </div>
                                  </div>";
                    }
                }
            }
            else
            {
                htmlContent += "<div class=\"text-center text-muted small py-3\">No conversation history yet.</div>";
            }

            return Content(htmlContent, "text/html");
        }
    }
}