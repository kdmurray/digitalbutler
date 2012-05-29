using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

using OpenPop.Pop3;
using OpenPop.Mime;

namespace DigitalButler
{
    class MailClient
    {
        public string PopServer;
        public string SmtpServer;
        public bool EnableSSL;
        public int PopPort;
        public int SmtpPort;

        public string Username;
        public string Password;

        public Pop3Client client;

        public MailClient(string username, string password, string popserver, string smtpserver, int popport, int smtpport, bool enablessl)
        {
            Username = username;
            Password = password;

            PopServer = popserver;
            PopPort = popport;
            SmtpServer = smtpserver;
            SmtpPort = smtpport;
            EnableSSL = enablessl;

            client = new Pop3Client();
        }

        public void SendMessage(string From, string To, string Subject, string Body, List<string> Attachments)
        {
            var client = new SmtpClient(SmtpServer, SmtpPort)
            {
                Credentials = new NetworkCredential(Global.Username, Global.Password),
                EnableSsl = true
            };

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress(From);
            msg.To.Add(new MailAddress(To));
            msg.Subject = Subject;
            msg.Body = Body;
            msg.BodyEncoding = Encoding.ASCII;
            msg.IsBodyHtml = false;

            foreach (string att in Attachments)
            {
                msg.Attachments.Add(new Attachment(att));
            }

            client.Send(msg);
        }

        public List<Message> GetMessages()
        {
            if (!client.Connected)
            {
                client.Connect(PopServer, PopPort, EnableSSL);
                client.Authenticate(Username, Password);
            }


            int messageCount = client.GetMessageCount();
            List<Message> allMessasges = new List<Message>(messageCount);

            for (int i = 1; i <= messageCount; i++)
            {
                allMessasges.Add(client.GetMessage(i));
            }

            return allMessasges;
        }

        public int GetMessageCount()
        {
            if (!client.Connected)
            {
                client.Connect(PopServer, PopPort, EnableSSL);
                client.Authenticate(Username, Password);
            }


            return client.GetMessageCount();
        }

        public void DeleteMessage(int messageId)
        {
            if (!client.Connected)
            {
                client.Connect(PopServer, PopPort, EnableSSL);
                client.Authenticate(Username, Password);
            }

            client.DeleteMessage(messageId);
        }

        public void Disconnect()
        {
            if (client.Connected)
            {
                client.Disconnect();
            }
        }
    }
}
