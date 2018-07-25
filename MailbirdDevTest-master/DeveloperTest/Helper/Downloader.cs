using DeveloperTest.Service;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperTest.Helper
{
    internal static class Downloader
    {
        const int MaxConcurrentTask = 5;
        private static readonly LimitedConcurrencyLevelTaskScheduler _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(MaxConcurrentTask);

        public static Task AddTaskToQueue(Action action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, _taskScheduler);
        }

    }
}
