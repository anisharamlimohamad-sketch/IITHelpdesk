using ItHelpdesk.Models;
using ITHelpdesk.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ItHelpdesk.Controllers
{
    public class UserPortalController : Controller
    {
        public IActionResult UserPortal() => View("~/Views/Helpdesk/UserPortal.cshtml");

        [HttpPost]
        public IActionResult SubmitIncident(IncidentReportViewModel model)
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            int todayCount = DataStore.Incidents
                .Count(i => i.Id != null && i.Id.StartsWith(currentDate));

            int nextNumber = todayCount + 1;
            string formattedNumber = nextNumber.ToString("D2");

            model.Id = $"{currentDate}-{formattedNumber}";

            if (string.IsNullOrWhiteSpace(model.Status))
            {
                model.Status = "New";
            }

            if (string.IsNullOrWhiteSpace(model.AdminComments))
            {
                model.AdminComments = "";
            }

            model.ActionLogs = new List<IncidentActionLog>();

            DataStore.Incidents.Add(model);

            TempData["SuccessMessage"] = $"Success! Support ticket {model.Id} submitted for Employee ID: {model.EmployeeId}.";

            return RedirectToAction("UserPortal");
        }

        [HttpPost]
        public IActionResult SubmitMis(MisRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = "NGK-" + new Random().Next(1000, 9999);
                }

                DataStore.MisRequests.Add(model);
                TempData["MisSuccess"] = $"Request logged successfully! Requester ID: {model.RequesterEmployeeId}.";
            }
            return RedirectToAction("UserPortal");
        }

        [HttpPost]
        public IActionResult MarkChatAsRead(string id)
        {
            var incident = DataStore.Incidents.FirstOrDefault(i => i.Id == id);
            if (incident != null)
            {
                incident.HasUnreadMessages = false;
            }
            return Ok();
        }
    }
}