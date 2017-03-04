using HeartWavesSDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class StopRecordRequest
    {
        public string type { get; set; }
        public string id { get; set; }
        public List<string> hrvdata { get; set; }
        public List<string> epdata { get; set; }
        public List<string> IBIdata { get; set; }
        public List<string> pulsedata { get; set; }
        public string rkind { get; set; }
        public string s_time { get; set; }
        public string time_length { get; set; }
        public string synthesisscore { get; set; }
        public string deflatingindex { get; set; }
        public string stabilityindex { get; set; }
        public string pressureindex { get; set; }
        public string HRVscore { get; set; }
        public string evaluation { get; set; }
        public List<string> HRVmark { get; set; }
        public string NB { get; set; }
        public string fMean { get; set; }
        public string fStdDev { get; set; }
        public string fSDNN { get; set; }
        public string fRMSSD { get; set; }
        public string fSD { get; set; }
        public string fSDSD { get; set; }
        public string fPNN { get; set; }
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
        public string trid { get; set; }
        public string tid { get; set; }
        public string gate { get; set; }
        public string trainscore { get; set; }
        public string diff { get; set; }
        public string FreData { get; set; }
        public string Level { get; set; }
        public string TimeLength { get; set; }
        public string RecordType { get; set; }
        public string Report { get; set; }

        public StopRecordRequest()
        {
            hrvdata = new List<string>();
            epdata = new List<string>();
            IBIdata = new List<string>();
            pulsedata = new List<string>();
            HRVmark = new List<string>();
        }

    }
}
