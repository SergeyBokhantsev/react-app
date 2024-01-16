using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Grpc.Core.Metadata;

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

        public static async Task<IDictionary<string, int>> SearchTextInZip(Stream stream, string[] fileExtensions, string[] texts)
        {
            Dictionary<string, int> result = new();

            async Task SearchInFile(ZipArchiveEntry entry, Stream unzippedStream)
            {
                using (var reader = new StreamReader(unzippedStream))
                {
                    var line = await reader.ReadLineAsync();
                    if (null != line)
                    {
                        foreach (var text in texts)
                        {
                            if (line.Contains(text, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!result.ContainsKey(entry.FullName))
                                    result.Add(entry.FullName, 1);
                                else
                                    result[entry.FullName]++;
                            }
                        }
                    }
                }
            }

            await Unzip(stream, entry =>
            {
                if (fileExtensions.Any(ext => entry.FullName.EndsWith(ext)))
                    return (e, s) => SearchInFile(e, s);
                else
                    return null;
            });

            return result;
        }

        public static async Task<Stream?> Unzip(Stream stream, string nameStartsWith)
        {
            var buffer = new byte[1024 * 1000];
            var outStream = new MemoryStream();
            bool fileUnzipped = false;

            async Task ReadUnzippedFileStream(Stream unzippedStream)
            {
                while (true)
                {
                    int len = await unzippedStream.ReadAsync(buffer, 0, buffer.Length);

                    if (len == 0)
                    {
                        fileUnzipped = true;
                        break;
                    }

                    await outStream.WriteAsync(buffer, 0, len);
                }
            }

            await Unzip(stream, entry =>
            {
                if (entry.FullName.StartsWith(nameStartsWith))
                    return (e, s) => ReadUnzippedFileStream(s);
                else
                    return null;
            });
            
            if (!fileUnzipped)
                return null;

            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }

        private static async Task Unzip(Stream stream, Func<ZipArchiveEntry, Func<ZipArchiveEntry, Stream, Task>?> predicate)
        {
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var taskFunc = predicate(entry);

                    if (null != taskFunc)
                    {
                        using (var ZipStream = entry.Open())
                        {
                            await taskFunc(entry, ZipStream);
                        }
                    }
                }
            }
        }
    }
}
