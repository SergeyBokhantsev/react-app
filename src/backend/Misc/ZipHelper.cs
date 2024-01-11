using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Misc
{
    public static class ZipHelper
    {
        public static class KnownPath
        {
            public const string JobLog = "Logs\\job.log";
            public const string SyncDump = "Logs\\SyncDump\\sync_dump_";
            public const string MasterSite = "Logs\\Configuration\\Master_Site.xml";
            public const string ExchSlaveSite = "Logs\\Configuration\\ExchangeSlave_Site.xml";
        }

        public static async Task<Stream?> Unzip(Stream stream, string nameStartsWith)
        {
            var buffer = new byte[1024 * 1000];
            var outStream = new MemoryStream();
            bool fileUnzipped = false;

            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.StartsWith(nameStartsWith))
                    {
                        using (var ZipStream = entry.Open())
                        {
                            while (true)
                            {
                                int len = await ZipStream.ReadAsync(buffer, 0, buffer.Length);

                                if (len == 0)
                                {
                                    fileUnzipped = true;
                                    break;
                                }

                                await outStream.WriteAsync(buffer, 0, len);
                            }
                        }
                    }
                }

                if (!fileUnzipped)
                    return null;
            }

            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }
    }
}
