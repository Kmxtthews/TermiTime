using System;
using System.Linq;
using System.Text.Json;

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

        public static class Globals
        {
            public static Users? CachedUser { get; set; }
        }

        public class Entry
        {
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        public class Theme
        {
            public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
            public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

            public void ApplyTheme()
            {
                Console.BackgroundColor = this.BackgroundColor;
                Console.ForegroundColor = this.ForegroundColor;
                Console.Clear();
            }
        }

        public static void SaveUserToFile(Users updatedUser)
        {
            string[] lines = File.ReadAllLines("user_data.json");
            bool found = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedLine = lines[i].Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                try
                {
                    JsonDocument doc = JsonDocument.Parse(trimmedLine);
                    JsonElement root = doc.RootElement;
                    string? username = root.GetProperty("Username").GetString();
                    string? password = root.GetProperty("Password").GetString();

                    if (username == updatedUser.Username && password == updatedUser.Password)
                    {
                        // Serialize the fully updated user object back to a single-line JSON
                        string updatedJson = JsonSerializer.Serialize(updatedUser, new JsonSerializerOptions { WriteIndented = false });
                        lines[i] = updatedJson;
                        found = true;

                        // Update cache too (just in case)
                        Helper.Globals.CachedUser = updatedUser;
                        break;
                    }
                }
                catch (JsonException)
                {
                    continue;
                }
            }

            if (found)
            {
                File.WriteAllLines("user_data.json", lines);
            }
            else
            {
                // This shouldn't happen, but safety
                Console.WriteLine("Error: Could not save theme — user not found in file.");
            }
        }
    }
}