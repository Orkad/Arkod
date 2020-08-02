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
        private bool looping = false;
        public FtpFileSystemWatcher(string server, ICredentials credentials)
        {
            this.server = server;
            this.credentials = credentials;
            files = GetFtpFiles();
            looping = true;
            new Thread(Loop).Start();
        }

        private Dictionary<string, string> GetFtpFiles()
        {
            var detailSet = new HashSet<string>();
            var detailedResponse = CallFtpMethod(WebRequestMethods.Ftp.ListDirectoryDetails);
            using (var detailedResponseReader = new StreamReader(detailedResponse.GetResponseStream()))
            {
                while (!detailedResponseReader.EndOfStream)
                {
                    detailSet.Add(detailedResponseReader.ReadLine());
                }
            }

            Dictionary<string, string> result = new Dictionary<string, string>();
            var fileResponse = CallFtpMethod(WebRequestMethods.Ftp.ListDirectory);
            using (var fileResponseReader = new StreamReader(fileResponse.GetResponseStream()))
            {
                while (!fileResponseReader.EndOfStream)
                {
                    var fileLine = fileResponseReader.ReadLine();
                    foreach (var detail in detailSet)
                    {
                        if (detail.EndsWith(fileLine))
                        {
                            result.Add(fileLine, detail);
                            break;
                        }
                    }
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

        private void Loop()
        {
            var delta = 0;
            while (looping)
            {
                var sleep = Frequency - delta;
                if(sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
                var sw = new Stopwatch();
                sw.Start();
                HandleFiles(); // <<< Main method
                sw.Stop();
                delta = (int)sw.ElapsedMilliseconds;
            }
        }

        private void HandleFiles()
        {
            // change tracking
            files.TrackChanges(GetFtpFiles(), true, out var changed, out var added, out var removed);
            foreach (var change in changed)
            {
                if (EnableRaisingEvents)
                {
                    Changed?.Invoke(this, new FtpFileSystemEventArgs { ChangeType = FtpWatcherChangeTypes.Changed, FullPath = change.Key, Name = change.Key });
                }
            }
            foreach (var add in added)
            {
                if (EnableRaisingEvents)
                {
                    Created?.Invoke(this, new FtpFileSystemEventArgs { ChangeType = FtpWatcherChangeTypes.Created, FullPath = add.Key, Name = add.Key });
                }
            }
            foreach (var remove in removed)
            {
                if (EnableRaisingEvents)
                {
                    Deleted?.Invoke(this, new FtpFileSystemEventArgs { ChangeType = FtpWatcherChangeTypes.Deleted, FullPath = remove.Key, Name = remove.Key });
                }
            }
        }

        /// <summary>
        /// Gets or sets the path of the ftp directory to watch. Based on the root directory
        /// </summary>
        [DefaultValue("")]
        [SettingsBindable(true)]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
        /// </summary>
        [DefaultValue("*.*")]
        [SettingsBindable(true)]
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component is enabled.
        /// </summary>
        [DefaultValue(false)]
        public bool EnableRaisingEvents { get; set; }

        /// <summary>
        /// Frequency in milliseconds used to call the FTP server (default: 1 second)
        /// </summary>
        [DefaultValue(1000)]
        public int Frequency { get; set; } = 1000;

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
                looping = false;
            }
            base.Dispose(disposing);
        }
    }
}
