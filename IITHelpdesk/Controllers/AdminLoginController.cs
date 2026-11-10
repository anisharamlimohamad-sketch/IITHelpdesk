using Microsoft.AspNetCore.Mvc;

namespace ItHelpdesk.Controllers
{
    public class AdminLoginController : Controller
    {
        public IActionResult Login()
        {
            return View("~/Views/Helpdesk/AdminLogin.cshtml");
        }

        [HttpPost]
        public IActionResult Authenticate(string username, string password)
        {
            if (username == "misadmin" && password == "mis2026")
            {
                return RedirectToAction("AdminDashboard", "AdminDashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Username or Password. Please try again.";
                return View("~/Views/Helpdesk/AdminLogin.cshtml");
            }
        }
    }
}