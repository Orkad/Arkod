using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;

namespace Arkod.Ftp.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FtpFileSystemWatcher()
        {
            var watcher = new FtpFileSystemWatcher("ftp://127.0.0.1:21", new NetworkCredential("test", "test"));
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            Thread.Sleep(60000);
        }

        private void Watcher_Changed(object sender, FtpFileSystemEventArgs e)
        {
            Console.WriteLine(e.Name + " changed");
        }

        private void Watcher_Deleted(object sender, FtpFileSystemEventArgs e)
        {
            Console.WriteLine(e.Name + " deleted");
        }

        private void Watcher_Created(object sender, FtpFileSystemEventArgs e)
        {
            Console.WriteLine(e.Name + " created");
        }
    }
}
