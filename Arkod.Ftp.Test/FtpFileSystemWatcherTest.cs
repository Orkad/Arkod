using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            var changed = new HashSet<string>();
            var added = new HashSet<string>();
            var removed = new HashSet<string>();
            // Create the fake ftp server
            using (var fakeFtp = new FtpTestServer(FTP_SERVER, FTP_PORT))
            {
                var credential = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                // Watch the ftp server
                using (var watcher = new FtpFileSystemWatcher(FTP_ADRESS, credential))
                {
                    watcher.Changed += (e, arg) => changed.Add(arg.FullPath);
                    watcher.Created += (e, arg) => added.Add(arg.FullPath);
                    watcher.Deleted += (e, arg) => removed.Add(arg.FullPath);

                    var fileToAdd = CreateTmpFile();
                    // make actions on the ftp
                    using (var client = new WebClient())
                    {
                        client.Credentials = credential;
                        var fileName = "SuperFile.tmp";
                        client.UploadFile(new Uri(new Uri(FTP_ADRESS), fileName), WebRequestMethods.Ftp.UploadFile, fileToAdd);
                    }
                }
            }
            Thread.Sleep(60000);
            Check.That(added).HasSize(1);
        }

        private static string CreateTmpFile()
        {
            var fileName = Path.GetTempFileName();
            FileInfo fileInfo = new FileInfo(fileName);
            fileInfo.Attributes = FileAttributes.Temporary;
            return fileName;
        }
    }
}
