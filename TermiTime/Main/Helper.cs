using System;
using System.Linq;

namespace Main
{
    internal class Helper
    {
        private static readonly string AllowedSpecials = "!?#$%^&*()-+={}@;";

        public static bool Validation(string username, bool capCheck = true, bool lenCheck = true, bool specialCheck = true)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            bool hasCapital = !capCheck || username.Any(char.IsUpper);
            bool hasMinLength = !lenCheck || username.Length >= 3;
            bool hasSpecialChar = !specialCheck || username.Any(c => AllowedSpecials.Contains(c));

            return hasCapital && hasMinLength && hasSpecialChar;
        }

        /// <summary>
        /// Displays an error message and waits for Enter
        /// </summary>
        public static void DisplayErrorAndWait(string message)
        {
            Console.WriteLine($"\n{message}");
            Console.WriteLine("Press Enter to try again...");
            Console.ReadLine();
        }
    }
}