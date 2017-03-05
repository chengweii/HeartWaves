using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PmtsControlLibrary
{

    class ApplicationPath
    {
        private readonly static string ImagePath = Environment.CurrentDirectory + "\\image\\";
        private readonly static string RtfPath = Environment.CurrentDirectory + "\\rtf\\";

        XmlDocument xDoc = new XmlDocument();
        public ApplicationPath()
        {
            xDoc.Load(getAppConfigPath());
        }

        public static string getImagePath()
        {
            return ImagePath;
        }

        public static string getRtfPath()
        {
            return RtfPath;
        }

        public string getAppConfigPath()
        {
            return Environment.CurrentDirectory + "\\App.config";
        }

        //获取App.config值  
        public string GetRtfPath(string appKey)
        {
            try
            {
                XmlNode xNode;
                XmlElement xElem;
                xNode = xDoc.SelectSingleNode("//appSettings");
                xElem = (XmlElement)xNode.SelectSingleNode("//add[@key='" + appKey + "']");
                if (xElem != null)
                {
                    return xElem.GetAttribute("value");
                }
                else
                    return "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

}
