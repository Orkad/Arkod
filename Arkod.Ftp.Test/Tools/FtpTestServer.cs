using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Arkod.Ftp.Test
{
    public sealed class FtpTestServer : IDisposable
    {
        private readonly string FtpDirectory;
        private readonly Process ftpProcess;

        public FtpTestServer(string server, int port = 21)
        {
            FtpDirectory = Path.Combine(Path.GetTempPath(), "FtpTestServer");
            if (!Directory.Exists(FtpDirectory))
            {
                Directory.CreateDirectory(FtpDirectory);
            }
            var ftpdminExeLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\ftpdmin.exe");
            ftpProcess = Process.Start(ftpdminExeLocation, @$"-p {port} -ha {server} ""{FtpDirectory}""");
            Thread.Sleep(50); // wait for process correctly starting
            if (ftpProcess.HasExited)
            {
                throw new InvalidOperationException($"Impossible de lancer correctement le processus ftpdmin");
            }
        }

        public void Dispose()
        {
            if (!ftpProcess.HasExited)
            {
                ftpProcess.Kill();
                ftpProcess.WaitForExit();
            }
            if (Directory.Exists(FtpDirectory))
            {
                Directory.Delete(FtpDirectory, true);
            }
        }
    }
}
