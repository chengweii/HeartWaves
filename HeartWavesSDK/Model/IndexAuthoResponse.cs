using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class IndexAuthoResponse
    {
        public string status { get; set; }
        public IndexAuthoData data { get; set; }

        public IndexAuthoResponse()
        {
            data = new IndexAuthoData();
        }
    }

    public class IndexAuthoDatum
    {
        public string id { get; set; }
        public string name { get; set; }
        public string fid { get; set; }
    }

    public class IndexAuthoData
    {
        public List<IndexAuthoDatum> data { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public IndexAuthoData()
        {
            data = new List<IndexAuthoDatum>();
        }
    }  
}
