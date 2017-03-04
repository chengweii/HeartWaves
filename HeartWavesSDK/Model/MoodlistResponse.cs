using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class MoodlistResponse
    {
        public string status { get; set; }

        public MoodlistData data { get; set; }

        public MoodlistResponse()
        {
            data = new MoodlistData();
        }
    }

    public class MoodlistData
    {
        public List<MoodlistDatum> result { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public MoodlistData()
        {
            result = new List<MoodlistDatum>();
        }
    }

    public class MoodlistDatum
    {
        public string moodtime { get; set; }
        public string moodmark { get; set; }
        public string moodsocre { get; set; }
    }
}
