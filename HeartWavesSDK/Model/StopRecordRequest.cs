using HeartWavesSDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HeartWavesSDK.Model
{
    public class StopRecordRequest
    {
        public string type { get; set; }
        public string id { get; set; }
        public ArrayList hrvdata { get; set; }
        public ArrayList epdata { get; set; }
        public ArrayList IBIdata { get; set; }
        public ArrayList pulsedata { get; set; }
        public string rkind { get; set; }
        public string s_time { get; set; }
        public string time_length { get; set; }
        public string synthesisscore { get; set; }
        public string deflatingindex { get; set; }
        public string stabilityindex { get; set; }
        public string pressureindex { get; set; }
        public string HRVscore { get; set; }
        public string evaluation { get; set; }
        public ArrayList HRVmark { get; set; }
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
        public string HeartVitalityUpper { get; set; }
        public string HeartVitalityDowner { get; set; }
        public string TimeType { get; set; }
        public string EndTime { get; set; }
        public string Mood { get; set; }

        public StopRecordRequest()
        {
            hrvdata = new ArrayList();
            epdata = new ArrayList();
            IBIdata = new ArrayList();
            pulsedata = new ArrayList();
            HRVmark = new ArrayList();
        }

    }
}
