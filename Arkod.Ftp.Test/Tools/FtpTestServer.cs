﻿using FubarDev.FtpServer;
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
            FtpDirectory = Path.Combine(Path.GetTempPath(), "FtpTestServer", Guid.NewGuid().ToString());
            var ftpdminExeLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\ftpdmin.exe");
            var processStartInfo = new ProcessStartInfo
            {
                FileName = ftpdminExeLocation,
                Arguments = @$"-p {port} -ha {server} ""{FtpDirectory}""",
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            //Vista or higher check
            if (Environment.OSVersion.Version.Major >= 6)
            {
                processStartInfo.Verb = "runas";
            }
            ftpProcess = Process.Start(processStartInfo);
            if (ftpProcess.HasExited)
            {
                throw new InvalidOperationException($"Impossible de lancer correctement le processus ftpdmin : {ftpProcess.StandardOutput.ReadToEnd()}");
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(FtpDirectory))
            {
                Directory.Delete(FtpDirectory, true);
            }
            if (!ftpProcess.HasExited)
            {
                ftpProcess.Kill();
                ftpProcess.WaitForExit();
            }
        }
    }
}
