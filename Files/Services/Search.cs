using Files.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Services
{
    public static class Search
    {
        private static CancellationTokenSource TokenSource = new();
        private static CancellationToken CToken = TokenSource.Token;
        public static void Stop() => TokenSource.Cancel();

        /// <summary>
        /// Start searching
        /// </summary>
        /// <param name="root">Root directory to search</param>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="progressAction">Progress action</param>
        public static async void AsyncSearch(string root, string searchPattern, Action<FileDetails> progressAction)
        {
            Stop();
            Thread.Sleep(10);

            TokenSource = new();
            CToken = TokenSource.Token;

            var progress = new Progress<FileDetails?>(file => progressAction(file));

            try
            {
                await Task.Run(() => Searching(root, searchPattern, progress), TokenSource.Token);
            }
            catch { }
        }

        private static void Searching(string root, string searchPattern, IProgress<FileDetails?> progress)
        {
            string pattern = '*' + searchPattern + '*';

            Stack<string> pending = new Stack<string>();
            pending.Push(root);

            while (pending.Count != 0)
            {
                if (CToken.IsCancellationRequested)
                {
                    //progress "null" is treated as the end of the task
                    progress.Report(null);
                    CToken.ThrowIfCancellationRequested();
                }

                var path = pending.Pop();
                string[]? next = null;
                try
                {
                    next = Directory.GetFiles(path, pattern).Concat
                        (Directory.GetDirectories(path, pattern)).ToArray();
                }
                catch { }

                if (next != null && next.Length != 0)
                {
                    foreach (var file in next)
                        progress.Report(GetFiles.GetFileDetails(file));
                }

                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next)
                        pending.Push(subdir);
                }
                catch { }
            }

            //progress "null" is treated as the end of the task
            progress.Report(null);
        }
    }
}
