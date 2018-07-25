using DeveloperTest.Commands;
using DeveloperTest.Interfaces;
using DeveloperTest.Models;
using DeveloperTest.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeveloperTest.ViewModels
{
    public class MainPageViewModel : BaseNotifyPropertyChangedModel
    {
        CancellationTokenSource cts;
        IEmailService emailService;
        private object obj = new object();
        private List<object> DownloadFailedList = new List<object>();

        #region Bindable Properties

        bool _isbusy;
        public bool IsBusy
        {
            get { return _isbusy; }
            set { SetProperty(ref _isbusy, value); }
        }

        string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        string _server;
        public string Server
        {
            get { return _server; }
            set { SetProperty(ref _server, value); }
        }

        string _serverType;
        public string ServerType
        {
            get { return _serverType; }
            set { SetProperty(ref _serverType, value); }
        }

        string _encryption;
        public string Encryption
        {
            get { return _encryption; }
            set { SetProperty(ref _encryption, value); }
        }

        string _messageBody;
        public string MessageBody
        {
            get { return _messageBody; }
            set { SetProperty(ref _messageBody, value); }
        }

        string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        int _port;
        public int Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }

        EmailItem _selectedItem;
        public EmailItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ICommand ConnectCommand { get; private set; }
        #endregion

        public ObservableCollection<EmailItem> EmailItems { get; set; }

        public MainPageViewModel()
        {
            EmailItems = new ObservableCollection<EmailItem>();
            ServerType = Constants.ServerTypes.IMAP;
            Encryption = Constants.Encryptions.SSL_TLS;
            Port = 993;
            cts = new CancellationTokenSource();
            ConnectCommand = new CommandHandler(OnConnectCommandExecutedAsync);
        }

        private IEnumerable<List<long>> SplitList(List<long> locations, int size)
        {
            for (int i = 0; i < locations.Count; i += size)
            {
                yield return locations.GetRange(i, Math.Min(size, locations.Count - i));
            }
        }

        private async void OnItemSelected(EmailItem item)
        {
            if (item == null)
                return;
            if (string.IsNullOrWhiteSpace(item.Body))
            {
                if (DownloadFailedList.Contains(item.Uid))
                    await DownloadEmailBodyAsync(item);
            }
            else
            {
                RenderEmailBodyToUI(item.Body);
            }
        }

        private void RenderItemsToGridView(IEnumerable<EmailItem> items)
        {
            lock (obj)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var item in items)
                    {
                        EmailItems.Add(item);
                    }
                });
            }
        }

        private void RenderEmailBodyToUI(string message)
        {
            lock (obj)
            {
                MessageBody = message;
            }
        }

        private void RenderErrorMessageToUI(string message)
        {
            lock (obj)
            {
                ErrorMessage = message;
            }
        }

        private bool Validate()
        {
            ErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Please enter username.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter Password.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Server))
            {
                ErrorMessage = "Please enter Server.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(ServerType))
            {
                ErrorMessage = "Please select ServerType.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Encryption))
            {
                ErrorMessage = "Please select Encryption.";
                return false;
            }
            else if (Port <= 0)
            {
                ErrorMessage = "Invalid Port Number.";
                return false;
            }
            return true;
        }

        private async void OnConnectCommandExecutedAsync()
        {
            if (IsBusy)
                return;
            if (!Validate())
                return;

            if (ServerType == Constants.ServerTypes.IMAP)
            {
                emailService = new ImapEmailService(Server, Username, Password, Encryption, Port);
            }
            else if (ServerType == Constants.ServerTypes.POP3)
            {
                emailService = new Pop3EmailService(Server, Username, Password, Encryption, Port);
            }
            else
            {
                ErrorMessage = "Selected server type is not supported.";
                return;
            }
            emailService.ItemsDownloaded += OnEmailService_ItemsDownloaded;
            IsBusy = true;
            try
            {
                EmailItems.Clear();
                await emailService.BeginDownloadInboxAsync(cts.Token);
            }
            catch (Exception ex)
            {
                RenderErrorMessageToUI(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnEmailService_ItemsDownloaded(object sender, List<EmailItem> items)
        {
            // Add Items to UI List
            RenderItemsToGridView(items);
            // Download Body
            foreach (var item in items)
            {
                DownloadEmailBodyAsync(item).NoAwait();
            }
        }

        private Task DownloadEmailBodyAsync(EmailItem item)
        {
            return Helper.Downloader.AddTaskToQueue(() =>
            {
                item.Body = emailService.DownloadBody(item.Uid);
                if (SelectedItem != null && SelectedItem.Uid == item.Uid)
                {
                    RenderEmailBodyToUI(item.Body);
                }
                else
                {
                    DownloadFailedList.Add(item.Uid);
                }
            }, cts.Token);
        }

        public void Cancel()
        {
            cts?.Cancel();
        }

    }

    public static class Extensions
    {
        public static void NoAwait(this Task task)
        {

        }
    }


    //private async void DownloadMessageBodyAsync(IEnumerable<EmailItem> items)
    //{
    //    await Task.Run(() =>
    //    {
    //        Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (item) =>
    //        {
    //            DownloadBody(item);
    //        });
    //    }, cts.Token);

    //}
    //private async Task DownloadUidsAsync(List<long> uids)
    //{
    //    try
    //    {
    //        // DIVIDE TO CHUNK
    //        var ids = uids.Select(x=>x)

    //        int curIndex = 0;
    //        var throttler = new SemaphoreSlim(initialCount: 2);
    //        var allTasks = new List<Task>();
    //        while (curIndex < allUids.Count)
    //        {
    //            await throttler.WaitAsync();
    //            var uids = allUids.Skip(curIndex).Take(QueueSize).ToList();
    //            curIndex += QueueSize;
    //            allTasks.Add(
    //                Task.Run(() =>
    //                {
    //                    var items = emailService.GetInboxEmailItems(uids);
    //                    AddToList(items);
    //                })
    //            );
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ErrorMessage = ex.Message;
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}
    //private async void DownloadMessageBodyAsync(EmailItem item)
    //{
    //    try
    //    {
    //        item.Body = await Task.Run(() => emailService.GetInboxEmailBody(item.Uid), cts.Token);
    //        if (SelectedItem.Uid == item.Uid)
    //        {
    //            RenderMessageToUI(item.Body);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ErrorMessage = ex.Message;
    //    }
    //}
}
