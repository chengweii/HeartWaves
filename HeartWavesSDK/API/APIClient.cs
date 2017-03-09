using HeartWavesSDK.Common;
using HeartWavesSDK.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartWavesSDK.API
{
    public class APIClient : APIBase
    {

        #region 登录

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static LoginResponse _Login(LoginRequest request)
        {
#if DEBUG
            request = new LoginRequest()
            {
                username = "bbb",
                password = "456789"
            };
#endif
            return Post<LoginResponse>("login", request);
        }

        #endregion

        #region 修改用户信息

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CommonResponse _EditMessage(EditMessageRequest request)
        {
#if DEBUG
            request = new EditMessageRequest()
            {
                id = "1",
                username = "bbb",
                birthday = "1991/02/24",
                email = "aaa@126.com",
                height = "180",
                medicalHistory = "无",
                mobile = "14741591142",
                position = "教员",
                sex = "2",
                weight = "70",
                workingPlace = "北京市朝阳区青年汇104号楼"
            };
#endif
            return Post<CommonResponse>("editmessage", request);
        }

        #endregion

        #region 获取权限列表

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IndexAuthoResponse _IndexAutho(IndexAuthoRequest request)
        {
#if DEBUG
            request = new IndexAuthoRequest()
            {
                id = "1"
            };
#endif
            return Post<IndexAuthoResponse>("indexautho", request);
        }

        #endregion

        #region 记录心情

        /// <summary>
        /// 记录心情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CommonResponse _RecordMood(MoodRequest request)
        {
#if DEBUG
            request = new MoodRequest()
            {
                user_id = "1",
                moodsocre = "50",
                moodremark = "测试"
            };
#endif
            return Post<CommonResponse>("mood", request);
        }

        #endregion

        #region 获取心情列表

        /// <summary>
        /// 获取心情列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MoodlistResponse _GetMoodlist(MoodlistRequest request)
        {
#if DEBUG
            request = new MoodlistRequest()
            {
                user_id = "1",
            };
#endif
            return Post<MoodlistResponse>("moodlist", request);
        }

        #endregion

        #region 获取公告栏

        /// <summary>
        /// 获取公告栏
        /// </summary>
        /// <returns></returns>
        public static NoticeboardResponse _GetNoticeboard()
        {
            return Get<NoticeboardResponse>("noticeboard", null);
        }

        #endregion

        #region 记录列表

        /// <summary>
        /// 记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static GetRecordResponse _GetRecord(GetRecordRequest request)
        {
#if DEBUG
            request = new GetRecordRequest()
            {
                user_id = "1",
                type = "1",
                pageNum = "0"
            };
#endif
            return Post<GetRecordResponse>("getrecord", request);
        }

        #endregion

        #region 停止记录

        /// <summary>
        /// 停止记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static StopRecordResponse _StopRecord(StopRecordRequest request)
        {
#if DEBUG
            request = new StopRecordRequest()
            {
                id = "1",
                type = "1"
            };

            request.hrvdata.Add("10");
            request.hrvdata.Add("50");

            request.epdata.Add("10");
            request.epdata.Add("50");

            request.IBIdata.Add("10");
            request.IBIdata.Add("50");

            request.pulsedata.Add("10");
            request.pulsedata.Add("50");

            request.HRVmark.Add("666");
            request.HRVmark.Add("888");
#endif
            return Post<StopRecordResponse>("stoprecord", request);
        }

        #endregion

        #region 获取记录详情

        /// <summary>
        /// 获取记录详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RecordDetailResponse _GetRecordDetail(RecordDetailRequest request)
        {
#if DEBUG
            request = new RecordDetailRequest()
            {
                user_id = "1",
                r_id = "48"
            };
#endif
            return Get<RecordDetailResponse>("recorddetail", request);
        }

        #endregion

        #region 删除记录

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CommonResponse _DeleteRecord(DeleteRecordRequest request)
        {
#if DEBUG
            request = new DeleteRecordRequest()
            {
                user_id = "1",
                r_id = "41,42"
            };
#endif
            return Post<CommonResponse>("deleterecord", request);
        }

        #endregion

        #region 上传雷达数据
        /// <summary>
        /// 上传雷达数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static UploadRadarDatasResponse _UploadRadarDatas(UploadRadarDatasRequest request)
        {
#if DEBUG
            request = new UploadRadarDatasRequest()
            {
                user_id = "1",
                observe = "200",
                rember = "500",
                emotion = "600",
                willpower = "800",
                thinking = "900"
            };
#endif
            return Post<UploadRadarDatasResponse>("radardatas", request);
        }

        #endregion
        
        #region 获取二维码
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="r_id"></param>
        /// <returns></returns>
        public static string _GetDex(string user_id,string r_id)
        {
#if DEBUG
		user_id = "1";
		r_id = "1";
#endif
			string uri=m_APIBase+"/hahaha/dex.php";
			if(!string.IsNullOrEmpty(user_id)&&!string.IsNullOrEmpty(r_id)){
				uri+="?user_id="+user_id+"&r_id="+r_id;
			}
			var content = SDKHttpRequest._GET(uri);
            return content;
        }

        #endregion
    }
}
