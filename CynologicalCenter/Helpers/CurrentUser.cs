using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Helpers
{
    public static class CurrentUser
    {
        public static string Username { get; private set; } = string.Empty;
        public static string Role { get; private set; } = string.Empty;
        public static bool IsAdmin => Role == "admin";
        public static bool IsOperator => Role == "operator" || Role == "admin";
        public static bool IsGuest => Role == "guest";

        public static void Login(string username, string role)
        {
            Username = username;
            Role = role;
        }

        public static void Logout()
        {
            Username = string.Empty;
            Role = string.Empty;
        }
    }
}
