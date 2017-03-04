using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class MoodRequest
    {
        public string user_id { get; set; }

        public string moodsocre { get; set; }

        public string moodremark { get; set; }
    }
}
