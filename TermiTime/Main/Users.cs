using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main;


namespace Main
{
    internal class Users
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public  DateTime CreationDate { get; set; }
        public List<String> Entries { get; set; }

        // This is the one System.Text.Json will use
        public Users() { }

        public Users(string Username_, string Password_)
        {
            Username = Username_;
            Password = Password_;
            CreationDate = DateTime.UtcNow;
        }

        public string GetCredentials()
        {
            return ($"Username={Username}Password={Password}");
        }
    }
}
