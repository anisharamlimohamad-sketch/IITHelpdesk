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
            // 1. Dapatkan tarikh hari ini dalam format yyyyMMdd (Contoh: 20261106)
            string currentDate = DateTime.Now.ToString("yyyyMMdd");

            // 2. Kira berapa banyak tiket yang sudah wujud pada hari yang sama
            int todayCount = DataStore.Incidents
                .Count(i => i.Id != null && i.Id.StartsWith(currentDate));

            // 3. Tambah 1 untuk nombor siri seterusnya dan formatkan jadi 2 digit (01, 02, 03...)
            int nextNumber = todayCount + 1;
            string formattedNumber = nextNumber.ToString("D2");

            // 4. Set ID mengikut format baharu: yyyymmdd-01
            model.Id = $"{currentDate}-{formattedNumber}";

            if (string.IsNullOrWhiteSpace(model.Status))
            {
                model.Status = "New";
            }

            if (string.IsNullOrWhiteSpace(model.AdminComments))
            {
                model.AdminComments = "";
            }

            // Pastikan senarai ActionLogs tidak null
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