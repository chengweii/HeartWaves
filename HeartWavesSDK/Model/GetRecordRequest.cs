using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartWavesSDK.Model
{
    public class GetRecordRequest
    {
        /// <summary>
        /// user_id	string	用户id	必填
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// pageNum	string	数据开始id	（0 10 20 以此类推）
        /// </summary>
        public string pageNum { get; set; }
        
        /// <summary>
        /// type string	1 监测记录 2 训练记录 3 放松记录
        /// </summary>
        public string type { get; set; }
    }
}
