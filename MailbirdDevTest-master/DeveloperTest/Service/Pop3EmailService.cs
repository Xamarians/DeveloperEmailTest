using DeveloperTest.Helper;
using DeveloperTest.Interfaces;
using DeveloperTest.Models;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperTest.Service
{

    public class Pop3EmailService : IEmailService
    {
        private readonly string Host;
        private readonly string Username;
        private readonly string Password;
        private readonly string Encryption;
        private readonly int PortNumber;
        public event EventHandler<List<EmailItem>> ItemsDownloaded;

        public Pop3EmailService(string host, string username, string password, string encryption, int port)
        {
            Host = host;
            Username = username;
            Password = password;
            Encryption = encryption;
            PortNumber = port;
        }

        private void ConnectAndLogin(Pop3 imap)
        {
            if (Encryption == Constants.Encryptions.SSL_TLS)
            {
                imap.ConnectSSL(Host, PortNumber);
            }
            else if (Encryption == Constants.Encryptions.START_TLS)
            {
                imap.Connect(Host, PortNumber);
                imap.StartTLS();
            }
            else
            {
                imap.Connect(Host, PortNumber);
            }
            imap.UseBestLogin(Username, Password);
        }

        private EmailItem ParseIMail(string uid, IMail email)
        {
            return new EmailItem(uid, email.From[0].Address, email.Subject, email.Date);
        }

        public Task BeginDownloadInboxAsync(CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
             {
                 using (var pop3 = new Pop3())
                 {
                     ConnectAndLogin(pop3);
                     var allUids = pop3.GetAll();
                     var chunkUids = Common.SplitList(allUids, 5);
                     var builder = new MailBuilder();

                     foreach (var uids in chunkUids)
                     {
                         var items = new List<EmailItem>();
                         foreach (var uid in uids)
                         {
                             var bytes = pop3.GetHeadersByUID(uid);
                             IMail email = builder.CreateFromEml(bytes);
                             items.Add(ParseIMail(uid, email));
                         }
                         ItemsDownloaded?.Invoke(this, items);
                     }
                 }
             }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public string DownloadBody(object uid)
        {
            using (var pop3 = new Pop3())
            {
                ConnectAndLogin(pop3);
                IMail email = new MailBuilder().CreateFromEml(pop3.GetMessageByUID(uid.ToString()));
                return email.Text;
            }
        }

    }
}
