using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CynologicalCenter.Helpers
{
    public static class RoleAccess
    {
        public static bool CanView =>
            CurrentUser.Role is "admin" or "operator" or "guest";

        public static bool CanManageClients =>
            CurrentUser.Role is "admin" or "operator";

        public static bool CanDeactivateClients =>
            CurrentUser.Role == "admin";

        public static bool CanManageSessions =>
            CurrentUser.Role is "admin" or "operator";

        public static bool CanManageTrainers =>
            CurrentUser.Role is "admin" or "operator";

        public static bool IsAdmin =>
            CurrentUser.Role == "admin";

        public static bool IsGuest =>
            CurrentUser.Role == "guest";

        public static bool IsOperatorOrAbove =>
            CurrentUser.Role is "admin" or "operator";

        public static bool CanViewReports =>
            CurrentUser.Role == "admin";

        public static bool CanViewBasicReports =>
            CurrentUser.Role is "admin" or "operator";
    }
}
