using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalButler
{
    class Global
    {
        //Credentials for Mail Server
        public static string Username { get; set; }
        public static string Password { get; set; }

        //Mail Serivce Provider
        public static string MailService { get; set; }

        //Service display name
        public static string ServiceName { get; set; }

        //Time to wait (in minutes) between cycle starts
        public static int CycleDelay { get; set; }
    }
}
