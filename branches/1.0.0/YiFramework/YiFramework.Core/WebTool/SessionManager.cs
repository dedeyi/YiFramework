using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace YiFramework.Core
{
    /// <summary>
    /// 管理当前用户会话存储
    /// </summary>
   public class SessionManager
    {
        public static string CurrentKey = "CURRENT_USER";

        /// <summary>
        /// 添加会话信息，到Cache中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add<T>(string key, T value)
        {
            if (HttpContext.Current == null)
            { throw new Exception("HttpContext should not be null,may it's not a http request!"); }
            HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// 获取会话信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(string key)
            where T : class
        {
            if (HttpContext.Current == null)
            { throw new Exception("HttpContext should not be null,may it's not a http request!"); }
            return HttpContext.Current.Session[key] as T;
        }

        public static void Remove(string key)
        {
            if (HttpContext.Current == null)
            { throw new Exception("HttpContext should not be null,may it's not a http request!"); }
            HttpContext.Current.Session.Remove(key);
        }
    }
}
