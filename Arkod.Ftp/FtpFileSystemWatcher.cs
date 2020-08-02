using Arkod.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Arkod.Ftp
{
    public class FtpFileSystemWatcher : Component
    {
        private readonly string server;
        private readonly ICredentials credentials;
        private readonly Dictionary<string, string> files;
        private readonly Timer Timer;
        private readonly int FrequencyMs;
        private readonly object locker = new object();
        public FtpFileSystemWatcher(string server, ICredentials credentials)
        {
            this.server = server;
            this.credentials = credentials;
            files = GetFtpFiles();
            FrequencyMs = 1000;
            Timer = new Timer(s => HandleFiles(), null, 0, FrequencyMs);
        }

        private Dictionary<string, string> GetFtpFiles()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var fileResponse = CallFtpMethod(WebRequestMethods.Ftp.ListDirectory);
            var detailedResponse = CallFtpMethod(WebRequestMethods.Ftp.ListDirectoryDetails);
            using (var fileResponseReader = new StreamReader(fileResponse.GetResponseStream()))
            using (var detailedResponseReader = new StreamReader(detailedResponse.GetResponseStream()))
            {
                while (!(fileResponseReader.EndOfStream && detailedResponseReader.EndOfStream))
                {
                    var fileLine = fileResponseReader.ReadLine();
                    var detailedLine = detailedResponseReader.ReadLine();
                    result.Add(fileLine, detailedLine);
                }
            }
            return result;
        }

        private WebResponse CallFtpMethod(string method)
        {
            var ftpWebRequest = (FtpWebRequest)WebRequest.Create(server);
            ftpWebRequest.Credentials = credentials;
            ftpWebRequest.Method = method;
            return ftpWebRequest.GetResponse();
        }

        private void HandleFiles()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            lock (locker)
            {
                stopWatch.Stop();
                var lockedMs = stopWatch.ElapsedMilliseconds;
                if (lockedMs >= FrequencyMs)
                {
                    return;
                }
                // change tracking
                files.TrackChanges(GetFtpFiles(), true, out var changed, out var added, out var removed);
                foreach (var change in changed)
                {
                    Changed?.Invoke(this, new FtpFileSystemEventArgs { ChangeType = FtpWatcherChangeTypes.Changed, FullPath = change.Key, Name = change.Key });
                }
                foreach (var add in added)
                {
                    Created?.Invoke(this, new FtpFileSystemEventArgs { ChangeType = FtpWatcherChangeTypes.Created, FullPath = add.Key, Name = add.Key });
                }
                foreach (var remove in removed)
                {
                    Deleted?.Invoke(this, new FtpFileSystemEventArgs { ChangeType = FtpWatcherChangeTypes.Deleted, FullPath = remove.Key, Name = remove.Key });
                }
            }
        }

        [DefaultValue("")]
        [SettingsBindable(true)]
        public string Path { get; set; }

        [DefaultValue("*.*")]
        [SettingsBindable(true)]
        public string Filter { get; set; }

        /// <summary>
        /// Occurs when a ftp file in the specified <see cref="Path"/> that match the <see cref="Filter"/> is deleted.
        /// </summary>
        public event EventHandler<FtpFileSystemEventArgs> Deleted;

        /// <summary>
        /// Occurs when a ftp file in the specified <see cref="Path"/> that match the <see cref="Filter"/> is created.
        /// </summary>
        public event EventHandler<FtpFileSystemEventArgs> Created;

        /// <summary>
        /// Occurs when a ftp file in the specified <see cref="Path"/> that match the <see cref="Filter"/> is changed.
        /// </summary>
        public event EventHandler<FtpFileSystemEventArgs> Changed;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
