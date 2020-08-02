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
        private static FtpTestServer FtpServer;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            FtpServer = new FtpTestServer(FTP_SERVER, FTP_PORT);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            FtpServer.Dispose();
            FtpServer = null;
        }

        [TestMethod]
        public void FtpFileSystemWatcherCreatedEvent()
        {
            var changed = new HashSet<string>();
            var added = new HashSet<string>();
            var removed = new HashSet<string>();

            var fileName = "SuperFile.tmp";
            var credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
            // Watch the ftp server
            using (var watcher = new FtpFileSystemWatcher(FTP_ADRESS, credentials))
            {
                watcher.Changed += (e, arg) => changed.Add(arg.FullPath);
                watcher.Created += (e, arg) => added.Add(arg.FullPath);
                watcher.Deleted += (e, arg) => removed.Add(arg.FullPath);

                watcher.Frequency = 20; // ultra fast watching
                watcher.EnableRaisingEvents = true;
                Thread.Sleep(1000);

                // make actions on the ftp
                // ADD A FILE
                var fileToAdd = CreateTmpFile();
                UploadFtpFile(credentials, FTP_ADRESS, fileName, File.OpenRead(fileToAdd));
                Thread.Sleep(50); // wait the event

                // CHANGE A FILE
                using (var writer = new StreamWriter(fileToAdd))
                {
                    writer.WriteLine("SuperFile content updated");
                }
                DeleteFtpFile(credentials, FTP_ADRESS, fileName);
                UploadFtpFile(credentials, FTP_ADRESS, fileName, File.OpenRead(fileToAdd));
                Thread.Sleep(50); // wait the event

                // REMOVE A FILE
                DeleteFtpFile(credentials, FTP_ADRESS, fileName);
                Thread.Sleep(50); // wait the event
            }

            Check.That(added).Contains(fileName);
            Check.That(changed).Contains(fileName);
            Check.That(removed).Contains(fileName);
        }

        private static string CreateTmpFile()
        {
            var fileName = Path.GetTempFileName();
            FileInfo fileInfo = new FileInfo(fileName);
            fileInfo.Attributes = FileAttributes.Temporary;
            return fileName;
        }

        private static void UploadFtpFile(ICredentials credentials, string adress, string fileName, Stream stream)
        {
            var request = (FtpWebRequest)WebRequest.Create(new Uri(new Uri(adress), fileName));
            request.Credentials = credentials;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            
            byte[] content;
            using (var reader = new StreamReader(stream))
            {
                content = Encoding.UTF8.GetBytes(reader.ReadToEnd());
            }
            request.ContentLength = content.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(content, 0, content.Length);
            }
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"UploadFtpFile Complete, status {response.StatusDescription}");
            }
        }

        private static void DeleteFtpFile(ICredentials credentials, string adress, string fileName)
        {
            var request = (FtpWebRequest)WebRequest.Create(new Uri(new Uri(adress), fileName));
            request.Credentials = credentials;
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine("DeleteFtpFile Complete, status " + response.StatusDescription);
            }
        }
    }
}
