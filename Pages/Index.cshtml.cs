﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using EllipticCurve.Utils;

namespace MyJobPortal.Pages
{

    // Sending sms
    public static class TwilioHelper
    {
        // Replace these with your Twilio account SID, auth token, and Twilio phone number
        private const string AccountSid = "ACd7cc01b48b08e606d1e5d771e1aec955";
        private const string AuthToken = "a9c6926ece1674548fec393a30d908e7";
        private const string TwilioPhoneNumber = "+19093462769";

        public static void SendSMS(string toPhoneNumber, string messageBody)
        {
            // Initialize the Twilio client
            TwilioClient.Init(AccountSid, AuthToken);

            try
            {
                // Send the SMS message
                var message = MessageResource.Create(
                    body: messageBody,
                    from: new Twilio.Types.PhoneNumber(TwilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );

                // Output the message SID (optional)
                Console.WriteLine($"SMS Sent successfully! Message SID: {message.Sid}");
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, notify, etc.)
                Console.WriteLine($"Error sending SMS: {ex.Message}");
            }
        }
    }
    public class Job
    {
        public int ID { get; set; }
        public string PositionName { get; set; }
        public string TeamName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public double Salary { get; set; }
        public string Experience { get; set; }
        public string JobDescription { get; set; }
        public string MinimumQualification { get; set; }
        public string LastDate { get; set; }
    }

    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string CompanySearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PositionSearch { get; set; }

        public List<Job> Jobs { get; set; }

        public IActionResult OnPost()
        {
            if (Request.Form.ContainsKey("applyButton"))
            {
                // Extract the job ID from the form submission
                var jobId = Convert.ToInt32(Request.Form["jobId"]);

                // Connect to the SQLite database
                var connectionString = "Data Source=/Users/arunachalamm/Projects/MyJobPortal/DB/JobPortal.db";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        // Build the SQL query to retrieve the selected job
                        command.CommandText = "SELECT * FROM Job WHERE ID = @JobId";
                        command.Parameters.AddWithValue("@JobId", jobId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Map the data from the database to the Job class
                                var selectedJob = new Job
                                {
                                    ID = reader.GetInt32(0),
                                    PositionName = reader.GetString(1),
                                    TeamName = reader.GetString(2),
                                    CompanyName = reader.GetString(3),
                                    Address = reader.GetString(4),
                                    Salary = reader.GetDouble(5),
                                    Experience = reader.GetString(6),
                                    JobDescription = reader.GetString(7),
                                    MinimumQualification = reader.GetString(8),
                                    LastDate = reader.GetString(9)
                                };

                                // Extract user details from the session
                                var userName = HttpContext.Session.GetString("UserName");
                                var userEmail = HttpContext.Session.GetString("UserEmail");

                                // Retrieve user's phone number from the database (replace this with your actual database query)
                                var userPhoneNumber = HttpContext.Session.GetString("UserPhone");

                                if (!string.IsNullOrEmpty(userPhoneNumber))
                                {
                                    // Construct the SMS message
                                    var messageBody = $"Dear {userName},\n\nYour application for the {selectedJob.PositionName} role at {selectedJob.CompanyName} has been submitted successfully. We appreciate your interest and will be reviewing your qualifications. Expect to hear from us soon regarding the next steps in the hiring process.\n\nThank you for considering {selectedJob.CompanyName}.\n\nBest regards,\nThe {selectedJob.CompanyName} Hiring Team";

                                    // Send the SMS
                                    TwilioHelper.SendSMS(userPhoneNumber, messageBody);
                                    var script = "alert('Confirmation SMS sent to registered mobile number.'); setTimeout(function(){ window.location.href = '/Index'; }, 1000);";
                                    Response.WriteAsync($"<script>{script}</script>");

                                    // Redirect to the same page (or any other page you want)
                                    return RedirectToPage("/Index");
                                }
                            }
                        }
                    }
                }
            }

            // Redirect to the same page (or any other page you want)
            return RedirectToPage("/Index");
        }

        public IActionResult OnGet()
        {
            // Connect to the SQLite database
            var connectionString = "Data Source=/Users/arunachalamm/Projects/MyJobPortal/DB/JobPortal.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Check if the Clear button is clicked
                    if (Request.Query.ContainsKey("clearSearch"))
                    {
                        // Clear the search parameters
                        CompanySearch = string.Empty;
                        PositionSearch = string.Empty;
                    }

                    // Build the SQL query based on the search parameters
                    var sqlQuery = "SELECT * FROM Job WHERE 1=1";

                    if (!string.IsNullOrEmpty(CompanySearch))
                    {
                        sqlQuery += " AND CompanyName LIKE '%" + CompanySearch + "%'";
                    }

                    if (!string.IsNullOrEmpty(PositionSearch))
                    {
                        sqlQuery += " AND PositionName LIKE '%" + PositionSearch + "%'";
                    }

                    command.CommandText = sqlQuery;

                    using (var reader = command.ExecuteReader())
                    {
                        Jobs = new List<Job>();

                        while (reader.Read())
                        {
                            // Map the data from the database to the Job class
                            var job = new Job
                            {
                                ID = reader.GetInt32(0),
                                PositionName = reader.GetString(1),
                                TeamName = reader.GetString(2),
                                CompanyName = reader.GetString(3),
                                Address = reader.GetString(4),
                                Salary = reader.GetDouble(5),
                                Experience = reader.GetString(6),
                                JobDescription = reader.GetString(7),
                                MinimumQualification = reader.GetString(8),
                                LastDate = reader.GetString(9)
                            };

                            // Add the job to the list
                            Jobs.Add(job);
                        }
                    }
                }
            }


            return Page();
        }
    }
}