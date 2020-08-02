using System;

namespace Arkod.Ftp
{
    public class FtpFileSystemEventArgs : EventArgs
    {
        public FtpWatcherChangeTypes ChangeType { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
    }
}
