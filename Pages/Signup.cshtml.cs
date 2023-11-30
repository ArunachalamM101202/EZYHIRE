using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

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

        private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourSecretKey123"); // 16, 24, or 32 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourIV1234567890"); // 16 bytes

        public static string Encrypt(string plainText)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
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


                var connectionString = "Data Source=/Users/arunachalamm/Desktop/MyJobPortal/DB/JobPortal.db";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        string tempname = Encrypt(Name);
                        string tempemail = Encrypt(Email);
                        string hash_password = ComputeHash(Password);
                        Console.WriteLine(tempname);
                        command.CommandText = "INSERT INTO Users (Name, Email, Password, Phone) VALUES (@Name, @Email, @Password, @PhoneNumber)";
                        command.Parameters.AddWithValue("@Name", tempname);
                        command.Parameters.AddWithValue("@Email", tempemail);
                        command.Parameters.AddWithValue("@Password", hash_password);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

                        command.ExecuteNonQuery();
                    }
                }
                // var script = "alert('Account Created Successfully!!'); setTimeout(function(){ window.location.href = '/Logout'; }, 1000);";

                // Response.WriteAsync($"<script>{script}</script>");
                // Redirect to a success page or another page as needed
                return RedirectToPage("/Login");
            }

            // If ModelState is not valid, redisplay the form
            return Page();
        }
    }
}
