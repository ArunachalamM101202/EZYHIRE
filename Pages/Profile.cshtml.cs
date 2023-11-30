using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;

namespace MyJobPortal.Pages
{
    public class ProfileModel : PageModel
    {
        // Existing properties...

        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }


        public IActionResult OnPost()
        {

            // Delete portion
            if (Request.Form.ContainsKey("deleteAccountButton"))
            {
                // Perform the account deletion (replace this with your actual deletion logic)
                var connectionString = "Data Source=/Users/arunachalamm/Desktop/MyJobPortal/DB/JobPortal.db";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM users WHERE Email = @UserEmail";
                        command.Parameters.AddWithValue("@UserEmail", HttpContext.Session.GetString("UserEmail"));

                        command.ExecuteNonQuery();
                    }
                }
                // var script = "alert('Account Deleted Successfully!!'); setTimeout(function(){ window.location.href = '/Logout'; }, 1000);";
                // Response.WriteAsync($"<script>{script}</script>");

                return RedirectToPage("/Logout");
            }

            // Update
            if (ModelState.IsValid)
            {
                // Fetch the current password from the database
                var connectionString = "Data Source=/Users/arunachalamm/Desktop/MyJobPortal/DB/JobPortal.db";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Password FROM Users WHERE Email = @UserEmail";
                        command.Parameters.AddWithValue("@UserEmail", HttpContext.Session.GetString("UserEmail"));

                        var currentPasswordFromDb = (string)command.ExecuteScalar();

                        // Check if the current password matches the one in the database
                        if (CurrentPassword == currentPasswordFromDb)
                        {
                            // Check if the new password and confirm password match
                            if (NewPassword == ConfirmPassword)
                            {
                                // Update the user's password in the database
                                command.CommandText = "UPDATE Users SET Password = @NewPassword WHERE Email = @UserEmail";
                                command.Parameters.AddWithValue("@NewPassword", NewPassword);

                                command.ExecuteNonQuery();

                                // Success message
                                // var script = "alert('Password updated successfully!!'); setTimeout(function(){ window.location.href = '/Profile'; }, 1000);";
                                // Response.WriteAsync($"<script>{script}</script>");

                                return RedirectToPage("/Logout");
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "New password and confirm password do not match.");
                                // Error message
                                var script = "alert('Passwords don't match!!'); setTimeout(function(){ window.location.href = '/Profile'; }, 1000);";
                                Response.WriteAsync($"<script>{script}</script>");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                        }
                    }
                }
            }

            // Return to the profile page
            return Page();
        }
    }
}
