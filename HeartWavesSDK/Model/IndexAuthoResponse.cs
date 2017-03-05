using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

    [DataContract]
    public class IndexAuthoDatum
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "is_is")]
        public string is_is { get; set; }

        [DataMember(Name = "0")]
        public List<IndexAuthoDatum> children { get; set; }

        public IndexAuthoDatum()
        {
            children = new List<IndexAuthoDatum>();
        }
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
