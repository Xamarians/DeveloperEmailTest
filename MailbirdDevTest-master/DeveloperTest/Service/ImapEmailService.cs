using DeveloperTest.Helper;
using DeveloperTest.Interfaces;
using DeveloperTest.Models;
using Limilabs.Client.IMAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperTest.Service
{
    public class ImapEmailService : IEmailService
    {
        private readonly string Host;
        private readonly string Username;
        private readonly string Password;
        private readonly string Encryption;
        private readonly int PortNumber;

        public event EventHandler<List<EmailItem>> ItemsDownloaded;

        public ImapEmailService(string host, string username, string password, string encryption, int port)
        {
            Host = host;
            Username = username;
            Password = password;
            Encryption = encryption;
            PortNumber = port;
        }

        private EmailItem ParseMessageInfoToEmail(MessageInfo info)
        {
            return new EmailItem(info.UID.Value, info.Envelope.From[0].Address, info.Envelope.Subject, info.Envelope.Date);
        }

        private void ConnectAndLogin(Imap imap)
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

        public Task BeginDownloadInboxAsync(CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var imap = new Imap())
                {
                    ConnectAndLogin(imap);
                    imap.SelectInbox();
                    var allUids = imap.GetAll();
                    var chunkUids = Common.SplitList(allUids, 5);
                    foreach (var uids in chunkUids)
                    {
                        List<MessageInfo> infos = imap.GetMessageInfoByUID(uids);
                        var items = infos.Select(x => ParseMessageInfoToEmail(x)).ToList();
                        ItemsDownloaded?.Invoke(this, items);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public string DownloadBody(object uid)
        {
            using (var imap = new Imap())
            {
                ConnectAndLogin(imap);
                imap.SelectInbox();
                var structure = imap.GetBodyStructureByUID(Convert.ToInt64(uid));
                string text = "";
                if (structure.Text != null)
                    text = imap.GetTextByUID(structure.Text);
                return text;
            }
        }

    }


}
