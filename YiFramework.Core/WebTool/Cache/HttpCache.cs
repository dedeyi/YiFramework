using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;

namespace YiFramework.Core
{
   public class HttpCache:ICacheStorage
    {
       private System.Web.Caching.Cache cache = HttpRuntime.Cache;

       private TimeSpan maxDuration = TimeSpan.FromDays(1);

       /// <summary>
       /// 最长持续时间
       /// </summary>
       public TimeSpan MaxDuration
       {
           get
           {
               return this.maxDuration;
           }
           set
           {
               this.maxDuration = value;
           }
       }

        public bool Add<T>(string key, T value)
        {
            return this.Add<T>(key, value, TimeSpan.FromHours(1));
        }

        public bool Add<T>(string key, T value, TimeSpan duration)
        {
            bool result = false;
            if (value != null)
            {
                this.cache.Add(key, value, null, DateTime.Now.Add(duration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
            return result;
        }

        public void Clear()
        {
            //	获取键集合
            IList<string> keys = new List<string>();
            IDictionaryEnumerator caches = this.cache.GetEnumerator();
            while (caches.MoveNext())
            {
                string key = caches.Key.ToString();
                keys.Add(key);
            }
            //	移除全部
            foreach (string key in keys)
            {
                this.cache.Remove(key);
            }
        }

        public T Get<T>(string key)
        {
            T result = default(T);
            object value = this.cache.Get(key);
            if (value is T)
            {
                result = (T)value;
            }
            return result;
        }

        public IDictionary<string, object> MultiGet(IList<string> keys)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in keys)
            {
                result.Add(key, this.Get<object>(key));
            }

            return result;
        }

        public void Remove(string key)
        {
            this.cache.Remove(key);
        }

        public bool Set<T>(string key, T value)
        {
            return this.Set<T>(key, value, TimeSpan.FromHours(1));
        }

        public bool Set<T>(string key, T value, TimeSpan duration)
        {
            bool result = false;
            if (value != null)
            {
                if (duration <= TimeSpan.Zero)
                {
                    duration = this.MaxDuration;
                }
                this.cache.Insert(key, value, null, DateTime.Now.Add(duration), System.Web.Caching.Cache.NoSlidingExpiration);
                result = true;
            }

            return result;
        }
    }
}
