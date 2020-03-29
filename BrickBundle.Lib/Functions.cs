using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BrickBundle.Lib
{
    public static class Functions
    {
        private static readonly PasswordHasher passwordHasher = new PasswordHasher(iterCount: 20000);

        public static byte[] HashPassword(string password)
        {
            return passwordHasher.HashPassword(password);
        }

        public static bool VerifyHashedPassword(byte[] hashedPassword, string password)
        {
            return passwordHasher.VerifyHashedPassword(hashedPassword, password);
        }

        public static bool IsValidEmail(string emailAddress)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(emailAddress);
                return address.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }

        private static readonly Regex usernameRegex = new Regex("^[a-zA-Z0-9_]+$");
        public static bool IsValidUsername(string username)
        {
            return username.Length >= 3 && usernameRegex.IsMatch(username);
        }

        public static bool IsValidPassword(string password)
        {
            return password.Length >= 6;
        }

        public static string GenerateCode(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
