using Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml;

namespace TermiTime
{
    // Main application class containing the program entry point and core logic
    internal class Core
    {
        // ASCII art title banner
        const string TitleString = " ______   ______     ______     __    __     __     ______   __     __    __     ______   \r\n/\\__  _\\ /\\  ___\\   /\\  == \\   /\\ \"-./  \\   /\\ \\   /\\__  _\\ /\\ \\   /\\ \"-./  \\   /\\  ___\\  \r\n\\/_/\\ \\/ \\ \\  __\\   \\ \\  __<   \\ \\ \\-./\\ \\  \\ \\ \\  \\/_/\\ \\/ \\ \\ \\  \\ \\ \\-./\\ \\  \\ \\  __\\  \r\n   \\ \\_\\  \\ \\_____\\  \\ \\_\\ \\_\\  \\ \\_\\ \\ \\_\\  \\ \\_\\    \\ \\_\\  \\ \\_\\  \\ \\_\\ \\ \\_\\  \\ \\_____\\\r\n    \\/_/   \\/_____/   \\/_/ /_/   \\/_/  \\/_/   \\/_/     \\/_/   \\/_/   \\/_/  \\/_/   \\/_____/";
        private static readonly (int width, int height) TitleSize = (90, 6);    // Dimensions of the title banner (used for console sizing)
        

        // Controls whether the title animation has a delay (useful for debugging)
        private static bool SkipLoadTime { get; set; } = true;

        /// <summary>
        /// Entry point of the application
        /// </summary>
        static void Main(string[] args)
        {

            ConsoleHelper.DisableResizing();        // ← Locks resizing completely
            Console.SetWindowSize(96, 30);          // Exact width your art needs
            Console.SetBufferSize(96, 30);          // Removes scrollbars
            Console.CursorVisible = false;          // Optional: hides blinking cursor

            Console.Title = "TermiTime";
            ConfigureConsole();

            ClearConsole();
            DisplayTitleAnimation();
            Console.WriteLine();
            Console.WriteLine("Welcome to TermiTime!\nAre you new here? (y/n)");

            bool unanswered = true;
            int invalidAttempts = 0;

            while (unanswered)
            {
                string? input = Console.ReadLine()?.Trim().ToLower();

                if (input == "y" || input == "yes")
                {
                    HandleNewUser();
                }
                else if (input == "n" || input == "no")
                {
                    HandleReturningUser();
                }
                else
                {
                    invalidAttempts++;
                    Console.WriteLine(invalidAttempts <= 1
                        ? "\nPlease answer with 'y' or 'n'."
                        : "\nInvalid input. Please type 'y' for yes or 'n' for no.");
                }
            }
        }

        /// <summary>
        /// Where I associate entries written by users to their list<string> entry field
        /// <summary>
        private static void SaveEntry(string content)
        {
            File.AppendAllText("entries.txt", content + Environment.NewLine);

            // Add entry to user's entry field
            Users OurUser = Helper.Globals.CachedUser;
            OurUser.Entries.Add(content);
        }


        /// <summary>
        /// Where users create new entries
        /// <summary>
        private static void CreateEntry()
        {
            ClearConsole();
            DisplayTitleAnimation();

            Console.WriteLine("\nThis is an entry, please type below.");
            Console.Write("\nEntry: ");
            string? entryContent = Console.ReadLine()?.Trim();

            Console.WriteLine("\nEntry saved! Press Enter to return to dashboard...");
            Console.ReadLine();

            SaveEntry(entryContent);

            // Return to dashboard using cached user object
            userDashboard(Helper.Globals.CachedUser);

        }


        /// <summary>
        /// Displays user dashboard according to credentials
        /// </summary>
        private static void userDashboard(Users? user)
        {
            ClearConsole();
            DisplayTitleAnimation();

            Helper.Globals.CachedUser = user!; // No need to serialize again, cached for session use

            bool inputLoop = true;

            while (inputLoop)
            {
                Console.WriteLine($"\n\nWelcome back {user?.Username}!\nPlease select one of the following.");
                Console.WriteLine("\n1. Create Entry\n2. View Entrys\n3. View Profile");

                string? input = Console.ReadLine()?.Trim();

                switch (input)
                {
                    case "1":
                        CreateEntry();
                        break;
                    case "2":
                        // View Entrys
                        break;
                    case "3":
                        // View Profile
                        break;
                }
            }
        }


        /// <summary>
        /// Allows returning users to log in
        /// </summary>
        private static void HandleReturningUser()
        {
            ClearConsole();
            DisplayTitleAnimation();
            Console.WriteLine("\nWelcome back!\nPlease enter your username...");
            Console.Write("Username :");

            string? username = Console.ReadLine()?.Trim();

            Console.WriteLine("\nNow enter your password!");
            Console.Write("Password :");

            string? password = Console.ReadLine()?.Trim();
            string userCheck = CheckForExistingUser(username);

            if (userCheck == "User already exists, account creation declined.")
            {
                // Account exists, check if password matches.

                string[] lines = File.ReadAllLines("user_data.json");

                foreach (string s in lines)
                {

                    Users? possibleUser = JsonSerializer.Deserialize<Users>(s);

                    // Safer to deserialize first, then compare fields

                    if (possibleUser?.Username == username && possibleUser?.Password == password)
                    {
                        try
                        {
                            Console.WriteLine("\nLogin successful! Press Enter to continue...");
                            userDashboard(possibleUser);
                            return;
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"An error occurred while accessing user data: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred while accessing user data: {ex.Message}");
                        }
                    }
                }

                Console.WriteLine("\nIncorrect password or username. Restarting login process, press enter....");
                Console.ReadLine();
                Main(new string[] { });
            }
        }

        /// <summary>
        /// Checks registered users for matching credentials
        /// </summary>
        private static string CheckForExistingUser(string? username)
        {
            // Check if input is not null
            if ((username is not null))
            {
                try
                {
                    string[] lines = File.ReadAllLines("user_data.json");

                    foreach (string s in lines)
                    {
                        if (s.Contains(username))
                        {
                            return "User already exists, account creation declined.";
                        }
                    }
                    return "User not found.";
                }
                catch (Exception ex)
                {
                    return $"An error occurred while accessing user data: {ex.Message}";
                }                
            } 
            else
            {
                return "Username or password was null.";
            }
        }

        /// <summary>
        /// Handles the new user registration flow
        /// </summary>
        private static void HandleNewUser()
        {
            string? username = PromptValidUsername();
            string? password = PromptValidPassword();

            ClearConsole();
            DisplayTitleAnimation();
            Console.WriteLine($"\nWelcome, {username}!\nJust checking some things...\nPress Enter to continue...");
            Console.ReadLine();

            string userCheck = CheckForExistingUser(username);

            if (userCheck == "User not found.")
            {
                //var options = new JsonSerializerOptions { WriteIndented = true }; // Pretty print option, not used here
                Users currentUser = new Users(username, password);
                string jsonString = JsonSerializer.Serialize(currentUser);
                string jsonLine = jsonString + Environment.NewLine; // adds /r/n on end
                File.AppendAllText("user_data.json", jsonLine);

                userDashboard(currentUser);
            }
            else
            {
                ClearConsole();
                DisplayTitleAnimation();
                Console.WriteLine($"\n{userCheck}");
                Console.WriteLine("Restarting Account creation process, press enter....");
                Console.ReadLine();

                Main(new string[] { });
            }
        }

        /// <summary>
        /// Prompts user for a valid username with validation rules
        /// </summary>
        private static string PromptValidUsername()
        {
            while (true)
            {
                ClearConsole();
                DisplayTitleAnimation();
                Console.WriteLine("\nLet's get you set up with an account!");
                Console.WriteLine("Username requirements:");
                Console.WriteLine(" • Must be longer than 3 characters");
                Console.WriteLine(" • Must contain at least one uppercase letter");
                Console.WriteLine(" • No special characters allowed");
                Console.Write("\nUsername: ");

                string? username = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(username))
                {
                    Helper.DisplayErrorAndWait("Username cannot be empty.");
                    continue;
                }

                if (Helper.Validation(username, capCheck: true, lenCheck: true, specialCheck: false))
                {
                    return username;
                }

                Helper.DisplayErrorAndWait("Invalid username. Please try again.");
            }
        }

        /// <summary>
        /// Prompts user for a valid password with strong requirements
        /// </summary>
        private static string PromptValidPassword()
        {
            while (true)
            {
                ClearConsole();
                DisplayTitleAnimation();
                Console.WriteLine("\nNow create a strong password.");
                Console.WriteLine("Requirements:");
                Console.WriteLine(" • At least 4 characters long");
                Console.WriteLine(" • At least one uppercase letter");
                Console.WriteLine(" • At least one special character: ! ? # $ % ^ & * ( ) - + = { } @ ;");
                Console.Write("\nPassword: ");

                string? password = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(password))
                {
                    Helper.DisplayErrorAndWait("Password cannot be empty.");
                    continue;
                }

                if (Helper.Validation(password, capCheck: true, lenCheck: true, specialCheck: true))
                {
                    return password;
                }

                Helper.DisplayErrorAndWait("Password does not meet requirements.");
            }
        }



        /// <summary>
        /// Configures console window and buffer size for consistent layout
        /// </summary>
        private static void ConfigureConsole()
        {
            try
            {
                // Minimize then resize to avoid scrollbars and ensure proper layout
                Console.SetWindowSize(1, 1);
                Console.SetBufferSize(TitleSize.width, 35); // Windows only
                Console.SetWindowSize(TitleSize.width, 35);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Fallback if screen is too small
                Console.SetWindowSize(Math.Min(TitleSize.width, Console.LargestWindowWidth), 40);
            }
        }

        /// <summary>
        /// Clears the console screen
        /// </summary>
        private static void ClearConsole() => Console.Clear();

        /// <summary>
        /// Displays the ASCII title with a typing animation effect
        /// </summary>
        private static void DisplayTitleAnimation()
        {
            foreach (char c in TitleString)
            {
                Console.Write(c);
                if (!SkipLoadTime)
                    Thread.Sleep(1); // Very fast animation; set SkipLoadTime = false to see effect
            }
        }

        /// <summary>
        /// Displays the static ASCII title without animation
        /// </summary>
        /// 

        //private static void DisplayTitle() => Console.WriteLine(TitleString);
    }
}