using System;
using System.Collections.Generic;
using System.Text;

namespace Baileysoft.Services.Ldap
{
    public class Settings
    {
        public static string Server = null;
        public static int Port = 389;
        public static string ServiceAccountDn = null;
        public static string ServiceAccountPassword = null;
        public static string SearchBase = null;
    }
}
