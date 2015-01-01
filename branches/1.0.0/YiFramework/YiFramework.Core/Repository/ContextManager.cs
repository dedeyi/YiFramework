using System;
using System.Diagnostics;

namespace YiFramework.Core
{
    /// <summary>
    /// Author:caoyi
    /// 数据上下文管理
    /// </summary>
    public class ContextManager
    {
        private const string suffix = "_TContext2014";
        private const string contextKeys = "ContextKeys";
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

            var item = ContextStorage.Get<TContext>(key);
            if (item == null)
            {
                var t = new TContext();
                Debug.WriteLine("数据上下文创建【time:" + DateTime.Now.Ticks + "），HashCode:" + t.GetHashCode() + "】");
                ContextStorage.Add(key, t);
                addContextKey(key);
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
            string[] allKeys = getContextKeys();
            if (allKeys != null && allKeys.Length > 0) 
            {
                foreach (string key in allKeys)
                {
                    IDisposable t = ContextStorage.Get<IDisposable>(key);
                    if (t != null) t.Dispose();
                    Debug.WriteLine("数据上下文释放【time:" + DateTime.Now.Ticks + "），HashCode:" + t.GetHashCode() + "】");
                }
            }
        }

        /// <summary>
        /// 是否制定类型上下文
        /// </summary>
        /// <param name="contextType"></param>
        public static void Dispose(Type contextType) 
        {
            if (contextType == null) throw new ArgumentNullException("对象不能为空");
            var tcontext = ContextStorage.Get<IDisposable>(contextType.ToString() + suffix);
            if (tcontext != null) {
                tcontext.Dispose();
                Debug.WriteLine("数据上下文释放【time:" + DateTime.Now.Ticks + "），HashCode:" + tcontext.GetHashCode() + "】");
            }
        }

        /// <summary>
        /// 存储上下文key
        /// </summary>
        /// <param name="key"></param>
        private static void addContextKey(string key) 
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key 不能为 空");
            string keys = ContextStorage.Get<string>(contextKeys);
            if (string.IsNullOrEmpty(keys))
            {
                keys = key;
            }
            else { keys += "," + key; }
            ContextStorage.Set(contextKeys, keys);
        }

        /// <summary>
        /// 获取上下文所有key
        /// </summary>
        private static string[] getContextKeys() 
        {
            string keys = ContextStorage.Get<string>(contextKeys);
            if (!string.IsNullOrEmpty(keys))
            {
                return keys.Split(',');
            }
            return null;
        }

    }
}
