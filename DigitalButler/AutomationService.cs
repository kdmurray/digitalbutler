using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using OpenPop.Mime;

namespace DigitalButler
{
    public partial class AutomationService : ServiceBase
    {
        bool ServiceIsActive = true;

        public AutomationService()
        {
            //InitializeComponent();

            //this.ServiceName = Global.ServiceName;
            //this.EventLog.Log = "Application";

            //this.CanHandlePowerEvent = true;
            //this.CanShutdown = true;
            //this.CanStop = true;

            LoadSettings();
        }

//        protected override void OnStart(string[] args)
        public new void OnStart(string[] args)
        {
            DateTime cycleStart;
            while (ServiceIsActive)
            {
                cycleStart = DateTime.Now;

                Console.WriteLine("Service is active. Beginning work cycle...");
                PerformWorkCycle();

                Console.WriteLine("Cycle completed. Beginning countdown to next cycle...");
                if (!(cycleStart.AddMinutes(Global.CycleDelay) < DateTime.Now))
                {
                    TimeSpan ts = DateTime.Now - cycleStart;

                    Console.WriteLine("Sleeping for {0} milliseconds...", ts.TotalMilliseconds);

                    Thread.Sleep(Convert.ToInt32(ts.TotalMilliseconds));
                }
                else
                {
                    //Default minimum time between runs
                    Thread.Sleep(1000);
                }
            }
        }

        private void PerformWorkCycle()
        {
            MailClient client;

            Console.WriteLine("Initializing mail client...");
            switch (Global.MailService)
            {
                case "Gmail":
                    client = new MailClient(Global.Username, Global.Password, Properties.Settings.Default.GmailPopServer, Properties.Settings.Default.GmailSmtpServer,Properties.Settings.Default.GmailPopPort,Properties.Settings.Default.GmailSmtpPort, true);
                    Console.WriteLine("Gmail client initialized.");
                    break;
                case "Yahoo":
                    client = new MailClient(Global.Username, Global.Password, Properties.Settings.Default.YahooPopServer, Properties.Settings.Default.YahooSmtpServer,Properties.Settings.Default.YahooPopPort,Properties.Settings.Default.YahooSmtpPort, true);
                    Console.WriteLine("Yahoo client initialized.");
                    break;
                case "Hotmail":
                    client = new MailClient(Global.Username, Global.Password, Properties.Settings.Default.HotmailPopServer, Properties.Settings.Default.HotmailSmtpServer,Properties.Settings.Default.HotmailPopPort,Properties.Settings.Default.HotmailSmtpPort, true);
                    Console.WriteLine("Hotmail client initialized.");
                    break;
                default:
                    throw new InvalidOperationException("Mail provider unknown.");
            }

            Console.WriteLine("Downloading messages...");

            List<Message> msgs = client.GetMessages();

            Console.WriteLine("Downloaded {0} messages.", msgs.Count);

            for (int i = 0; i < msgs.Count; i++)
            {
                Console.WriteLine("Processing message {0}...", i + 1);
                Message m = msgs[i];
                MessageType t = mp.DetermineMessageType(m);

                FreeCycleManager fcm;
                EpubManager epm;
                FileManager fmgr;

                Console.WriteLine("Processing message {0}...", i + 1);
                Message m = msgs[i];
                MessageType t = mp.DetermineMessageType(m);

                FreeCycleManager fcm;
                EpubManager epm;
                FileManager fmgr;

                switch (t)
                {
                    case MessageType.FreecycleOffer:
                        fcm = new FreeCycleManager();
                        fcm.ProcessOffer(m);
                        break;
                    case MessageType.FreecycleWanted:
                        fcm = new FreeCycleManager();
                        fcm.ProcessWanted(m);
                        break;
                    case MessageType.FreecycleTaken:
                        fcm = new FreeCycleManager();
                        fcm.ProcessTaken(m);
                        break;
                    case MessageType.EpubDecrypt:
                        epm = new EpubManager();
                        epm.Decrypt(m);
                        break;
                    case MessageType.GetDirectoryListing:
                        fmgr = new FileManager();
                        fmgr.GetDirectoryListing(m);
                        break;
                    case MessageType.GetFile:
                        fmgr = new FileManager();
                        fmgr.GetFile(m);
                        break;
                    case MessageType.Undetermined:
                        Console.WriteLine("    No match found. Type: Undetermined.");
                        break;
                    default:
                        break;
                }

                Console.WriteLine("    Deleting message....");
                client.DeleteMessage(i + 1);

            }

            client.Disconnect();
        }

        protected override void OnStop()
        {
            ServiceIsActive = false;
        }

        void LoadSettings()
        {
            Global.Username = (string)Properties.Settings.Default.Username;
            Global.Password = (string)Properties.Settings.Default.Password;
            Global.MailService = (string)Properties.Settings.Default.DefaultMailService;
            Global.CycleDelay = (int)Properties.Settings.Default.CycleDelay;
        }

    }
}
 