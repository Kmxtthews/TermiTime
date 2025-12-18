using Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using static Main.Helper;

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

        private static void Cleanup()
        {
            // Check if a user has been cached this session
            if (Helper.Globals.CachedUser is not null)
            {
                // A user is cached
                //
            }
        }

        /// <summary>
        /// Entry point of the application
        /// </summary>
        static void Main(string[] args)
        {

            // Minimize then resize to avoid scrollbars and ensure proper layout
            if (OperatingSystem.IsWindows())
            {
                ConsoleHelper.DisableResizing();        // ← Locks resizing completely
                Console.SetWindowSize(96, 30);          // Exact width your art needs
                Console.SetBufferSize(96, 30);          // Removes scrollbars
                Console.CursorVisible = false;          // Optional: hides blinking cursor
            }

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\nCleanup running before exit...");

                // Cleanup code
                Cleanup();
                e.Cancel = true;
                Environment.Exit(0);
            };

            Console.Title = "TermiTime";
            ConfigureConsole();

            ClearConsole();
            DisplayTitleAnimation();
            Console.WriteLine();
            Console.WriteLine("Welcome to TermiTime!\nAre you new here? (y/n)");


            //wasdwasd
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
        private static void Entry(string title, string content, string option, int passedInput)
        {
            string[] lines = File.ReadAllLines("user_data.json");
            Users OurUser = Helper.Globals.CachedUser!;
            bool found = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string trimmedLine = lines[i].Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                try
                {
                    // Parse the current line as a JSON object
                    JsonDocument doc = JsonDocument.Parse(trimmedLine);
                    JsonElement root = doc.RootElement;
                    string? username = root.GetProperty("Username").GetString();  // Adjust property names to match your JSON
                    string? password = root.GetProperty("Password").GetString();

                    if (username == OurUser.Username && password == OurUser.Password)
                    {
                        Users? fileUser = JsonSerializer.Deserialize<Users>(root.GetRawText());

                        // Add the new entry to the in-memory user object

                        if (option == "editing")
                        {
                            if (fileUser != null && passedInput >= 0 && passedInput < fileUser.Entries.Count)
                            {
                                fileUser.Entries[passedInput].Title = title;
                                fileUser.Entries[passedInput].Content = content;
                            }
                        }
                        else if (option == "delete")
                        {
                            fileUser?.Entries.RemoveAt(passedInput);
                            ClearConsole();
                            DisplayTitleAnimation();
                        }
                        else
                        {
                            fileUser?.Entries.Add(new Entry { Title = title, Content = content });
                        }

                        // Serialize the UPDATED OurUser object back to a single line JSON string
                        string updatedJson = JsonSerializer.Serialize(fileUser, new JsonSerializerOptions { WriteIndented = false });

                        // Replace the old line with the new serialized version
                        lines[i] = updatedJson;
                        found = true;

                        Helper.Globals.CachedUser = fileUser;
                        break;
                    }
                }
                catch (JsonException)
                {
                    // Skip invalid JSON lines
                    Debug.WriteLine("Invalid JSON encountered, skipping line.");
                    continue;
                }
            }

            if (found)
            {
                // Write all lines back to the file
                File.WriteAllLines("user_data.json", lines);
            }
            else
            {
                ClearConsole();

                Helper.Globals.CachedUser = null; // Log out user
                Console.WriteLine("An error occured\nUser data couldn't be accessed\nEntry deleted, logging user out.\nPress Enter...");
                Console.ReadLine();
                Main(new string[] { }); // Restart app
            }

        }

        private static void ChangeTheme()
        {
            ClearConsole();
            DisplayTitleAnimation();

            Users? cUser = Helper.Globals.CachedUser!;
            Console.WriteLine($"Current selection:\n    Background: {cUser.UserTheme?.BackgroundColor}\n    Foreground: {cUser.UserTheme?.ForegroundColor}\n\nSelect a terminal theme:\n\n1 •Dark Theme (\"Default\")\n2 •Light Theme\n3 •Solarized Theme\n4 •Midnight Theme\n5 •Return");
            string? input = Console.ReadLine()?.Trim();
            bool changed = false;

            switch (input)
            {
                case "1":
                    cUser.UserTheme!.BackgroundColor = ConsoleColor.Black;
                    cUser.UserTheme!.ForegroundColor = ConsoleColor.White;
                    cUser.UserTheme!.ApplyTheme();
                    changed = true;
                    break;
                case "2":
                    cUser.UserTheme!.BackgroundColor = ConsoleColor.White;
                    cUser.UserTheme!.ForegroundColor = ConsoleColor.Black;
                    cUser.UserTheme!.ApplyTheme();
                    changed = true;
                    break;
                case "3":
                    cUser.UserTheme!.BackgroundColor = ConsoleColor.DarkYellow;
                    cUser.UserTheme!.ForegroundColor = ConsoleColor.White;
                    cUser.UserTheme!.ApplyTheme();
                    changed = true;
                    break;
                case "4":
                    cUser.UserTheme!.BackgroundColor = ConsoleColor.DarkMagenta;
                    cUser.UserTheme!.ForegroundColor = ConsoleColor.Yellow;
                    cUser.UserTheme!.ApplyTheme();
                    changed = true;
                    break;
                case "5":
                        getProfile(true);
                    break;
                default:
                    Console.WriteLine("\nEnsure your input is valid in this context...\nPress enter if you understand...");
                    Console.ReadLine();
                    ChangeTheme();
                    break;
            }

            if (changed)
            {
                cUser.UserTheme!.ApplyTheme();
                Helper.SaveUserToFile(cUser);

                Console.WriteLine("\nTheme applied and saved! Press enter to return to profile...");
                Console.ReadLine();
                getProfile(true);
            }
        }

        private static void getProfile(bool useCached)
        {

            ClearConsole();
            DisplayTitleAnimation();
            Users cUser = Helper.Globals.CachedUser!;

            string? username = cUser.Username;
            string? password = cUser.Password;
            DateTime creationDate = cUser.CreationDate;
            int entryCount = cUser.Entries.Count;

            Console.WriteLine($"\nProfile Information:\n\nUsername: {username}\nPassword: {password}\nAccount Created: {creationDate}\nTotal Entries: {entryCount}");
            Console.WriteLine("\n1 •Change terminal theme\n2 •View Entries\n3 •Return");

            string? input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    ChangeTheme();
                    break;
                case "2":
                    viewEntrys(false, 0);
                    break;
                case "3":
                    ClearConsole();
                    DisplayTitleAnimation();
                    return;
                default:
                    Console.WriteLine("\nEnsure your input is valid in this context...\nPress enter if you understand...");
                    Console.ReadLine();
                    getProfile(true);
                    break;
            }

            return;
        }


        /// <summary>
        /// Where users create new entries
        /// <summary>
        private static void CreateEntry(bool editing, int passedInput)
        {
            ClearConsole();
            DisplayTitleAnimation();

            Console.WriteLine("\nPlease enter a title for this entry...");
            Console.Write("\nTitle: ");
            string? entryTitle = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(entryTitle))
            {
                Console.WriteLine("\nTitle cannot be empty! Press enter to try again...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nThis is the entry, please type below.");
            Console.Write("\nEntry: ");
            string? entryContent = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(entryContent))
            {
                Console.WriteLine("\nContent cannot be empty! Press enter to try again...");
                Console.ReadLine();
                return;
            }

            if (editing)
            {
                Entry(entryTitle!, entryContent!, "editing", passedInput);
            }
            else
            {
                Console.WriteLine("\nEntry saved! Press enter to return to the dashboard...");
                Console.ReadLine();
                Entry(entryTitle!, entryContent!, "save", -1);
            }

            // Return to dashboard using cached user object
            try
            {
                userDashboard(Helper.Globals.CachedUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("You have been logged out.\nEntry was not saved.\nPress Enter...");
                Console.ReadLine();
                Helper.Globals.CachedUser = null;
                Main(new string[] { });
            }
        }

        private static void displayEntry(string title, string content, bool skip, int passedInput)
        {
            ClearConsole();
            DisplayTitleAnimation();

            Console.WriteLine($"Title: {title}");

            foreach (char c in content)
            {
                Console.Write(c);
                if (!skip)
                    Thread.Sleep(1);
            }

            Console.WriteLine("\n\n1. Edit entry\n2. Delete entry\n3. Next entry");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CreateEntry(true, passedInput);
                    break;
                case "2":
                    Entry(title, content, "delete", passedInput);
                    break;
                case "3":
                    viewEntrys(true, passedInput);
                    break;
                default:
                    Console.WriteLine("\nEnsure your input is valid in this context...\nPress enter if you understand...");
                    Console.ReadLine();
                    ClearConsole();
                    displayEntry(title, content, true, passedInput);
                    break;
            }
        }

        private static void viewEntrys(bool nextEntry, int passedInput)
        {
            Users cUser = Helper.Globals.CachedUser!;

            if (nextEntry)
            {
                try
                {
                    int newInput = passedInput + 1;
                    displayEntry(cUser.Entries[newInput].Title, cUser.Entries[newInput].Content, false, newInput);
                    Debug.WriteLine("WORKEDDDDDD");
                }
                catch
                {
                    Console.WriteLine($"\nOut of range!, Press enter if understood.");
                    Console.ReadLine();
                    viewEntrys(false, 0);
                }
            }
            else
            {
                ClearConsole();
                DisplayTitleAnimation();

                
                Console.WriteLine($"Welcome {cUser.Username}!\nEnter a number corresponding with the entry you want to view.");
                // where list generates

                if (cUser.Entries.Count <= 0)
                {
                    Console.WriteLine("\nYou have no entries yet! Press enter to return to the dashboard...");
                    Console.ReadLine();
                    userDashboard(cUser);
                }
                else
                {
                    for (int i = 0; i < cUser.Entries.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {cUser.Entries[i].Title} ");
                    }
                }

                //Console.WriteLine("\nYou have no entries yet! Press enter to return to the dashboard...");
                //Console.ReadLine();
                //userDashboard(cUser);

                Console.Write("\nEntry : ");
                string input = Console.ReadLine()!.Trim();

                try
                {
                    int trueInput = int.Parse(input) - 1;
                    displayEntry(cUser.Entries[trueInput].Title, cUser.Entries[trueInput].Content, false, trueInput);
                }
                catch
                {
                    Console.WriteLine($"\nPlease ensure input is valid, press enter if understood.");
                    Console.ReadLine();
                    viewEntrys(false, 0);
                }
            }


        }

        /// <summary>
        /// Displays user dashboard according to credentials
        /// </summary>
        private static void userDashboard(Users? user)
        {
            user?.UserTheme?.ApplyTheme();
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
                        CreateEntry(false, -1);
                        break;
                    case "2":
                        viewEntrys(false, 0);
                        break;
                    case "3":
                        getProfile(true);
                        break;
                    default:
                        Console.WriteLine("\nEnsure your input is valid in this context...\nPress enter if you understand...");
                        Console.ReadLine();
                        ClearConsole();
                        DisplayTitleAnimation();
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
            Console.WriteLine("\nEnter x to start again.");
            Console.Write("Username :");

            string? username = Console.ReadLine()?.Trim();

            if (username == "x")
            {
                Console.WriteLine("Starting again! Press enter to continue.");
                Console.ReadLine();
                Main(new string[] { });
            }

            Console.WriteLine("\nNow enter your password!");
            Console.WriteLine("\nEnter x to start again.");
            Console.Write("Password :");

            string? password = Console.ReadLine()?.Trim();

            if (password == "x")
            {
                Console.WriteLine("Starting again! Press enter to continue.");
                Console.ReadLine();
                Main(new string[] { });
            }

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
            else if (userCheck == "User not found.")
            {
                Console.WriteLine("User could not be found in database. Press enter if understood.");
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
                Console.WriteLine("\n\nEnter x to start again.");
                Console.Write("\nUsername: ");

                string? username = Console.ReadLine()?.Trim();

                if (username == "x")
                {
                    Console.WriteLine("Starting again! Press enter to continue.");
                    Console.ReadLine(); 
                    Main(new string[] { }); 
                }

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


                if (password == "x")
                {
                    Console.WriteLine("Starting again! Press enter to continue.");
                    Console.ReadLine();
                    Main(new string[] { });
                }

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
                if (OperatingSystem.IsWindows())
                {
                    Console.SetWindowSize(1, 1);
                    Console.SetBufferSize(TitleSize.width, 35); // Windows only
                    Console.SetWindowSize(TitleSize.width, 35);
                }

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