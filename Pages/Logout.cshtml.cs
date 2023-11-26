using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyJobPortal.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Remove session variables during logout
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserEmail");

            // Redirect to the login page after logout
            return RedirectToPage("/Login");
        }
    }
}
