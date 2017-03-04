using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class GetRecordResponse
    {
        public string status { get; set; }
        public GetRecordData data { get; set; }

        public GetRecordResponse()
        {
            data = new GetRecordData();
        }
    }

    public class GetRecordDatum
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string rkind { get; set; }
        public string s_time { get; set; }
        public string time_length { get; set; }
        public string synthesisscore { get; set; }
        public string deflatingindex { get; set; }
        public string stabilityindex { get; set; }
        public string pressureindex { get; set; }
        public string hrvscore { get; set; }
        public string evaluation { get; set; }
        public string hrvmark { get; set; }
        public string epdata { get; set; }
        public string hrvdata { get; set; }
        public string ibidata { get; set; }
        public string pulsedata { get; set; }
        public string rtime { get; set; }
        public string type { get; set; }
        public string nb { get; set; }
        public string fmean { get; set; }
        public string fstddev { get; set; }
        public string fsdnn { get; set; }
        public string frmssd { get; set; }
        public string fsd { get; set; }
        public string fsdsd { get; set; }
        public string fpnn { get; set; }
        public string tp { get; set; }
        public string vlf { get; set; }
        public string lf { get; set; }
        public string hf { get; set; }
        public string lhr { get; set; }
        public string lfnorm { get; set; }
        public string hfnorm { get; set; }
        public string left { get; set; }
        public string right { get; set; }
        public string lrr { get; set; } 
    }

    public class GetRecordAutho
    {
        public string indextrend { get; set; }
        public string export { get; set; }
        public string screen { get; set; }
    }

    public class GetRecordData
    {
        public List<GetRecordDatum> data { get; set; }
        public GetRecordAutho autho { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public GetRecordData()
        {
            data = new List<GetRecordDatum>();
            autho = new GetRecordAutho();
        }
    }  
}
