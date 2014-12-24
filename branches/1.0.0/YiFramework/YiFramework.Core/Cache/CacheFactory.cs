using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YiFramework.Core
{
   public class CacheFactory
    {

       public static ICacheStorage GetCacheStorage() 
       {
           return new HttpCache();
       }
    }
}
