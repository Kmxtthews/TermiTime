using System;
using System.IO;
using Main;
using System.Threading;
using System.Collections.Generic;
using System.Xml;

namespace Main  // Console.ForegroundColor = ConsoleColor.Black;
{
    class Core
    {
        //  Constants
        const string TitleString = " ______   ______     ______     __    __     __     ______   __     __    __     ______   \r\n/\\__  _\\ /\\  ___\\   /\\  == \\   /\\ \"-./  \\   /\\ \\   /\\__  _\\ /\\ \\   /\\ \"-./  \\   /\\  ___\\  \r\n\\/_/\\ \\/ \\ \\  __\\   \\ \\  __<   \\ \\ \\-./\\ \\  \\ \\ \\  \\/_/\\ \\/ \\ \\ \\  \\ \\ \\-./\\ \\  \\ \\  __\\  \r\n   \\ \\_\\  \\ \\_____\\  \\ \\_\\ \\_\\  \\ \\_\\ \\ \\_\\  \\ \\_\\    \\ \\_\\  \\ \\_\\  \\ \\_\\ \\ \\_\\  \\ \\_____\\\r\n    \\/_/   \\/_____/   \\/_/ /_/   \\/_/  \\/_/   \\/_/     \\/_/   \\/_/   \\/_/  \\/_/   \\/_____/";
        public static readonly (int width, int height) TitleSize = (90, 6);

        static void Main(string[] args)
        {
            Console.Title = "TermiTime";
            int userOptionCount = 0;
            bool Unanswered = true;

            ClearConsole();
            ConsoleResize();
            DisplayTitleAnimation();

            while (Unanswered)
            {
                string userOption = Console.ReadLine();


                if (userOption.ToLower() == "y")
                {
                    // New user
                    Unanswered = false;
                    NewUser();
                }
                else if (userOption.ToLower() == "n")
                {
                    // Returning user
                    Unanswered = false;
                }
                else
                {
                    // Invalid input
                    userOptionCount++;
                    if (userOptionCount <= 1)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Please answer in the desired format! (y/n)");
                    }
                }
            }

            string user = Console.ReadLine();
            string pass = Console.ReadLine();

            Users newUser = new Users(user, pass);

            Console.WriteLine($"Credentials={newUser.GetCredentials()}");


        }

        static void NewUser() 
        {
            ClearConsole();
            DisplayTitle();
            Console.WriteLine("\nLet's get you set up with an account!");
            Console.WriteLine("Please enter a username, this is who we will refer to when talking to you!");


        }

        static void ConsoleResize()
        {
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(TitleSize.width, 60);
            Console.SetWindowSize(TitleSize.width, 60);
        }

        static void ClearConsole()
        {
            Console.Clear();
        }

        static void DisplayTitleAnimation()
        {
            foreach (char c in TitleString)
            {
                Console.Write(c);
                Thread.Sleep(1);
            }

            Console.WriteLine("");
            Console.WriteLine("Welcome to TermiTime!\nAre you new here? (y/n)");
        }

        static void DisplayTitle()
        {
            Console.WriteLine(TitleString);
        }
    }
}