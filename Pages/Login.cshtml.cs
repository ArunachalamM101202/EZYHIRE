using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http; // Add this for HttpContext.Session
using System.Threading.Tasks;

namespace MyJobPortal.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var connectionString = "Data Source=/Users/arunachalamm/Projects/MyJobPortal/DB/JobPortal.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id, Name, Email, Phone FROM Users WHERE Email = @Email AND Password = @Password";
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Password", Password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Successful login

                            // Store user information in session
                            HttpContext.Session.SetString("UserId", reader["Id"].ToString());
                            HttpContext.Session.SetString("UserName", reader["Name"].ToString());
                            HttpContext.Session.SetString("UserEmail", reader["Email"].ToString());
                            HttpContext.Session.SetString("UserPhone", reader["Phone"].ToString());

                            return RedirectToPage("/Index");
                        }
                        else
                        {
                            // Failed login
                            return RedirectToPage("/Privacy");
                        }
                    }
                }
            }
        }
    }
}
