using FunctionApp.Misc;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Azure
{
    public class SyncDumpService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SyncDumpService(IHttpClientFactory httpClientFactory) 
        {
            this.httpClientFactory = httpClientFactory;
        }

        public Task<Stream> DownloadFileAsync(Uri dumpUri)
        {
            var client = httpClientFactory.CreateClient(nameof(SyncDumpService));
            return client.GetStreamAsync(dumpUri);
        }

        public async Task<string> GetFile(Uri dumpUri, string filePath)
        {
            var fileStream = await DownloadFileAsync(dumpUri);

            var unzippedStream = await ZipHelper.Unzip(fileStream, filePath);

            if (null == unzippedStream)
                throw new Exception($"{filePath} file was not found in the Sync Dump archive");

            using (unzippedStream)
            using (var reader = new StreamReader(unzippedStream))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<IDictionary<string, int>> SearchInFiles(Uri dumpUri, string[] fileExtensions, string text)
        {
            var fileStream = await DownloadFileAsync(dumpUri);

            return await ZipHelper.SearchTextInZip(fileStream, fileExtensions, new[] { text });
        }
    }
}
