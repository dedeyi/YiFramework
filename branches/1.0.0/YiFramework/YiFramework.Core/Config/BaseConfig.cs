using System;
using System.Collections.Generic;
using System.Xml;

namespace YiFramework.Core
{

    /// <summary>
    /// 操作基础平台base.config配置文件的帮助类
    /// </summary>
    public class BaseConfig
    {
        private static Object thisLock = new Object();

        private static Dictionary<string, string> DicConfig;

        /// <summary>
        /// 初始化base.config配置文件
        /// </summary>
        public static void Init()
        {
                XmlTextReader xmlReader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "base.config");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                xmlReader.Close();
                XmlNodeList nodes = xmlDoc.SelectSingleNode("/appSettings").ChildNodes;
                DicConfig = new Dictionary<string, string>();
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null)
                    {
                        DicConfig.Add(node.Attributes["key"].Value, node.Attributes["value"].Value);
                    }
                }
                xmlReader.Dispose();
        }

        /// <summary>
        /// 根据key获取base.config配置文件的value值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            lock (thisLock)
            {
                return DicConfig[key];
            }
        }

    }
}
