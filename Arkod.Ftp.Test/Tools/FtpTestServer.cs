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
        private readonly Process ftpProcess;
        public readonly string ftpDirectory;

        [DebuggerNonUserCodeAttribute]
        public FtpTestServer(string server, int port = 21)
        {
            ftpDirectory = Path.Combine(Path.GetTempPath(), "FtpTestServer");
            if (!Directory.Exists(ftpDirectory))
            {
                Directory.CreateDirectory(ftpDirectory);
            }
            var ftpdminExeLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\ftpdmin.exe");
            var processInfo = new ProcessStartInfo
            {
                FileName = ftpdminExeLocation,
                Arguments = @$"-p {port} -ha {server} ""{ftpDirectory}""",
                RedirectStandardOutput = true,
            };
            ftpProcess = Process.Start(processInfo);
            Thread.Sleep(50); // wait for process correctly starting
            if (ftpProcess.HasExited)
            {
                throw new InvalidOperationException($"ftpdmin error : {ftpProcess.StandardOutput.ReadToEnd()}");
            }
        }

        public void Dispose()
        {
            if (!ftpProcess.HasExited)
            {
                ftpProcess.Kill();
                ftpProcess.WaitForExit();
            }
            if (Directory.Exists(ftpDirectory))
            {
                Directory.Delete(ftpDirectory, true);
            }
        }

        /// <summary>
        /// Simulate a file added on FTP
        /// </summary>
        /// <param name="path">Full path of the file on the FTP</param>
        /// <param name="stream">file content</param>
        public void AddFile(string path, Stream stream)
        {
            var ftpFile = Path.Combine(ftpDirectory, path);
            using var fileStream = File.Create(ftpFile);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
        }

        /// <summary>
        /// Simulate a file edition on FTP
        /// </summary>
        /// <param name="path">Full path of the file on the FTP</param>
        /// <param name="stream">new file content</param>
        public void ModifyFile(string path, Stream stream)
        {
            var ftpFile = Path.Combine(ftpDirectory, path);
            if (!File.Exists(ftpFile))
            {
                throw new FileNotFoundException("there is no file ");
            }
            using var fileStream = File.OpenWrite(ftpFile);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
        }

        /// <summary>
        /// Simulate a file deleted on FTP
        /// </summary>
        /// <param name="path">Full path of the file on the FTP</param>
        public void RemoveFile(string path)
        {
            var ftpFile = Path.Combine(ftpDirectory, path);
            if (File.Exists(ftpFile))
            {
                File.Delete(ftpFile);
            }
        }
    }
}
