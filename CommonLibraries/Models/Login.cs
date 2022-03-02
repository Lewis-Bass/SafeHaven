using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibraries.Models
{
    public class Login
    {
        public string Name { get; private set; }

        public string Password { get; private set; }

        public Login (string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
