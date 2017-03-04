using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class UploadRadarDatasRequest
    {
        public string user_id { get; set; }
        public string observe { get; set; }
        public string rember { get; set; }
        public string emotion { get; set; }
        public string willpower { get; set; }
        public string thinking { get; set; }
    }
}
