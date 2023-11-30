using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Http; // Add this for HttpContext.Session
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace MyJobPortal.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

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
            var connectionString = "Data Source=//Users/arunachalamm/Desktop/MyJobPortal/DB/JobPortal.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    string tempemail = Encrypt(Email);
                    string temppassword = ComputeHash(Password);

                    command.CommandText = "SELECT Id, Name, Email, Phone FROM Users WHERE Email = @Email AND Password = @Password";
                    command.Parameters.AddWithValue("@Email", tempemail);
                    command.Parameters.AddWithValue("@Password", temppassword);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Successful login

                            // Store user information in session
                            HttpContext.Session.SetString("UserId", reader["Id"].ToString());
                            HttpContext.Session.SetString("UserName", Decrypt(reader["Name"].ToString()));
                            HttpContext.Session.SetString("UserEmail", Decrypt(reader["Email"].ToString()));
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
