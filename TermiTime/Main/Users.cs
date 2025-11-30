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
        private string username;
        private string password;

        public Users(string username_, string password_)
        {
            this.username = username_;
            this.password = password_;
        }

        public string GetCredentials()
        {
            return ($"Username={this.username}Password={this.password}");
        }
    }
}
