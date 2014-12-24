using System;
using System.Collections;
using System.Data.Objects;
using System.Web;

namespace YiFramework.Core
{
    /// <summary>
    /// Author:caoyi
    /// EF上下文管理
    /// </summary>
    public class ContextManager
    {
        private const string suffix = "_TContext2014";
        /// <summary>
        /// Author:caoyi
        /// 在一个请求上下文中创建一次EF上下文实例
        /// </summary>
        /// <typeparam name="TContext">上下文对象</typeparam>
        /// <param name="context">上下文实例</param>
        /// <returns></returns>
        public static TContext Instance<TContext>() where TContext
            : class,IDisposable, new()
        {
            string key = typeof(TContext).ToString() + suffix;
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return new TContext();
                //throw new Exception("不存在HttpContext对象，无法完成上下文存储！"); 
            }

            var item = httpContext.Items[key] as TContext;
            if (item == null)
            {
                var t = new TContext();
                // Debug.WriteLine("EF创建时间（" + DateTime.Now.Ticks + "），HashCode（" + t.GetHashCode() + "）");
                httpContext.Items[key] = t;
                return t;
            }
            return item;
        }

        /// <summary>
        /// Author:caoyi
        /// 是否一次请求的所有创建的上下文
        /// </summary>
        public static void Dispose()
        {
            var httpContex = HttpContext.Current;
            IDictionary items = httpContex.Items;
            if (items == null || items.Count == 0 || items.Keys.Count == 0)
            {
                return;
            }

            foreach (var k in items.Keys)
            {
                if (k.ToString().IndexOf(suffix) > -1)
                {
                    var c = httpContex.Items[k] as ObjectContext;
                    if (c != null)
                    {
                        //  Debug.WriteLine("EF释放时间（"+DateTime.Now.Ticks+"），HashCode（"+c.GetHashCode()+"）");
                        c.Dispose();
                    }
                }
            }
        }
    }
}
