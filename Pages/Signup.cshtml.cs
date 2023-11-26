using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace MyJobPortal.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public required string Name { get; set; }

        [BindProperty]
        public required string Email { get; set; }

        [BindProperty]
        public required string Password { get; set; }

        [BindProperty]
        public required string ConfirmPassword { get; set; }

        [BindProperty]
        public required string PhoneNumber { get; set; }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Check if password and confirm password match
                if (Password != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password do not match.");
                    return Page();
                }


                var connectionString = "Data Source=/Users/arunachalamm/Projects/MyJobPortal/DB/JobPortal.db";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Users (Name, Email, Password, Phone) VALUES (@Name, @Email, @Password, @PhoneNumber)";
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

                        command.ExecuteNonQuery();
                    }
                }

                // Redirect to a success page or another page as needed
                return RedirectToPage("/Login");
            }

            // If ModelState is not valid, redisplay the form
            return Page();
        }
    }
}
