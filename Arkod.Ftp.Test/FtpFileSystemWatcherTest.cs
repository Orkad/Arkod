using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Arkod.Ftp.Test
{
    [TestClass]
    public class FtpFileSystemWatcherTest
    {
        private const int FTP_PORT = 21;
        private const string FTP_SERVER = "127.0.0.1";
        private static string FTP_ADRESS = $"ftp://{FTP_SERVER}:{FTP_PORT}";

        [TestMethod]
        public void FtpFileSystemWatcherCreatedEvent()
        {
            var fileName = "SuperFile.tmp";
            var credentials = new NetworkCredential("anonymous", "");
            var changed = new HashSet<string>();
            var added = new HashSet<string>();
            var removed = new HashSet<string>();

            using var ftpServer = new FtpTestServer(FTP_SERVER, FTP_PORT);
            using var watcher = new FtpFileSystemWatcher(FTP_ADRESS, credentials);
            watcher.Changed += (e, arg) => changed.Add(arg.FullPath);
            watcher.Created += (e, arg) => added.Add(arg.FullPath);
            watcher.Deleted += (e, arg) => removed.Add(arg.FullPath);
            watcher.Frequency = 500; // ultra fast watching
            watcher.EnableRaisingEvents = true;
            Thread.Sleep(1000);

            // ADD A FILE
            using (var stream = File.OpenRead(CreateTmpFile(fileName, "a little test")))
            {
                ftpServer.AddFile(fileName, stream);
            }
            Thread.Sleep(1500); // wait the event

            // CHANGE A FILE
            using (var stream = File.OpenRead(CreateTmpFile(fileName, "SuperFile content updated")))
            {
                ftpServer.AddFile(fileName, stream);
            }
            Thread.Sleep(1500); // wait the event

            // REMOVE A FILE
            ftpServer.RemoveFile(fileName);
            Thread.Sleep(1500); // wait the event

            Check.That(added).Contains(fileName);
            Check.That(changed).Contains(fileName);
            Check.That(removed).Contains(fileName);
        }

        private static string CreateTmpFile(string name, string content)
        {
            var path = Path.Combine(Path.GetTempPath(), name);
            using var writer = new StreamWriter(path);
            writer.Write(content);
            return path;
        }
    }
}
