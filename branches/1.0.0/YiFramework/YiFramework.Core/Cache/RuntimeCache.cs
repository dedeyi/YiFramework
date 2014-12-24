using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YiFramework.Core
{
    public class RuntimeCache : ICacheStorage
    {

        public bool Add<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public bool Add<T>(string key, T value, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> MultiGet(IList<string> keys)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T value, TimeSpan duration)
        {
            throw new NotImplementedException();
        }
    }
}
