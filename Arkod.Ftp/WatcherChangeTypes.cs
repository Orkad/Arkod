using System;

namespace Arkod.Ftp
{
    [Flags]
    public enum FtpWatcherChangeTypes
    {
        Created = 1,
        Deleted = 2,
        Changed = 4
    }
}
