using HeartWavesSDK.API;
using HeartWavesSDK.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PmtsControlLibrary
{
    public class UserInfoStatic
    {
        public UserInfoStatic()
        {
        }
        public static String UserID = "";
        public static String UserName = "";
        public static String UserSex = "男";
        public static String UserAge = "";///出生日期
        public static String UserWork = "未知";//职务
        public static int UserWorkYear = 0;
        public static String UserWorkType = "";
        public static String UserWorkArea = "";
        public static String UserMR = "";
        public static double O = 0f;
        public static double R = 0f;
        public static double T = 0f;
        public static double E = 0f;
        public static double W = 0f;
        public static double HRVS = 0f;
        public static String dbUser = "";
        public static String dbPwd = "";
        public static String dbName = "pmtsnet";
        public static String ipAdd = "";

        public static HeartWavesSDK.Model.UserInfo UserInfo { get; set; }

        private static List<IndexAuthoDatum> indexAuthoDatumList = null;

        public static bool hasAuth(string categoryName)
        {
            if (UserInfoStatic.ipAdd != null)
            {
                if (indexAuthoDatumList == null)
                {
                    var request = new IndexAuthoRequest()
                    {
                        id = UserInfoStatic.UserInfo.id
                    };
                    var response = APIClient._IndexAutho(request);
                    indexAuthoDatumList = response.data.data;
                }

                var category = findCategory(categoryName, indexAuthoDatumList);
                return indexAuthoDatumList != null && category != null;
            }
            return true;
        }

        private static IndexAuthoDatum findCategory(string categoryName, List<IndexAuthoDatum> categoryList)
        {
            IndexAuthoDatum result = null;
            foreach (var category in categoryList)
            {
                if (category.name.Equals(categoryName))
                {
                    result = category;
                    break;
                }
                else
                {
                    if (category.children != null)
                        result = findCategory(categoryName, category.children);
                    if (result != null)
                        break;
                }
            }
            return result;
        }
    }

}
