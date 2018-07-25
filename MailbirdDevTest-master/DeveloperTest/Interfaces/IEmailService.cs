using DeveloperTest.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperTest.Interfaces
{
    public interface IEmailService
    {
        //event EventHandler DownloadedCompleted;
        event EventHandler<List<EmailItem>> ItemsDownloaded;
        Task BeginDownloadInboxAsync(CancellationToken token);
        string DownloadBody(object uid);
    }

}
