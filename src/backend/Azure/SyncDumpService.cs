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

        public async Task<string> GetJobLogAsync(Uri dumpUri)
        {
            var fileStream = await DownloadFileAsync(dumpUri);

            var unzippedStream = await ZipHelper.Unzip(fileStream, ZipHelper.KnownPath.JobLog);

            if (null == unzippedStream)
                throw new Exception("job.log file was not found in the Sync Dump archive");

            using (unzippedStream)
            using (var reader = new StreamReader(unzippedStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
