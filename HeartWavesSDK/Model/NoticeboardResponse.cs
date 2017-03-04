using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class NoticeboardResponse
    {
        public string status { get; set; }

        public NoticeboardData data { get; set; }

        public NoticeboardResponse()
        {
            data = new NoticeboardData();
        }
    }

    public class NoticeboardData
    {
        public List<NoticeboardDatum> data { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public NoticeboardData()
        {
            data = new List<NoticeboardDatum>();
        }
    }

    public class NoticeboardDatum
    {
        public string content { get; set; }
    }
}
