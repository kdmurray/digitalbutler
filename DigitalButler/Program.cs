using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace DigitalButler
{
    static class Program
    {
        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //static void Main(string[] args)
        //{
        //    ServiceBase[] ServicesToRun;
        //    ServicesToRun = new ServiceBase[] 
        //    { 
        //        new AutomationService() 
        //    };
        //    ServiceBase.Run(ServicesToRun);
        //}



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AutomationService auto = new AutomationService();

            auto.OnStart(args);
        }


    }
}
