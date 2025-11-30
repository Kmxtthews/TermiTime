using Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
                    unanswered = false;
                    HandleNewUser();
                }
                else if (input == "n" || input == "no")
                {
                    unanswered = false;
                    Console.WriteLine("\nWelcome back! (Returning user logic not yet implemented)");
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
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
        /// Handles the new user registration flow
        /// </summary>
        private static void HandleNewUser()
        {
            string username = PromptValidUsername();
            string password = PromptValidPassword();

            ClearConsole();
            DisplayTitleAnimation();
            Console.WriteLine("\nAccount created successfully!");
            Console.WriteLine($"Welcome, {username}!");
            Console.WriteLine("Logging you in now...");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();

            // Create user object (assuming Users class exists in Helper or another file)
            var newUser = new Users(username, password);

            // TODO: Save user to file/database (JSON, XML, SQLite, etc.)
            // SaveUserToFile(newUser);
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
        /// Not used rn
        /// </summary>
        /// 

        //private static void DisplayTitle() => Console.WriteLine(TitleString);
    }
}