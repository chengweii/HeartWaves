using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.API
{
    public class UploadRadarDatasResponse
    {
        public string status { get; set; }

        public UploadRadarDatasData data { get; set; }

        public UploadRadarDatasResponse()
        {
            data = new UploadRadarDatasData();
        }
    }

    public class UploadRadarDatasData
    {
        public UploadRadarDatasDatum data { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public UploadRadarDatasData()
        {
            data = new UploadRadarDatasDatum();
        }
    }

    public class UploadRadarDatasDatum
    {
        public string id { get; set; }
        public string observe { get; set; }
        public string rember { get; set; }
        public string emotion { get; set; }
        public string willpower { get; set; }
        public string thinking { get; set; }
        public string ctime { get; set; }
        public string user_id { get; set; }
        public string hrvscore { get; set; }
    }
}
