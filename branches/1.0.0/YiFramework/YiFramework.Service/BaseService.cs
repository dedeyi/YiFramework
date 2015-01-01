using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YiFramework.Core;
using YiFramework.DBModel;

namespace YiFramework.Service
{
   public class BaseService
    {
          /// <summary>
      /// 缓存对象
      /// </summary>
      protected static ICacheStorage cacheStorage = CacheFactory.GetCacheStorage();

      /// <summary>
      /// Repository工厂
      /// </summary>
      public RepositoryFactory RepositoryFactory;

      /// <summary>
      /// 写入日志信息
      /// </summary>
      /// <param name="remark"></param>
      /// <param name="saveChange"></param>
      public void WriteLog(string remark, bool saveChange)
      {

      }

      /// <summary>
      /// 获取主键值
      /// </summary>
      /// <returns></returns>
      public string GetPrimaryKeyValue() { return Guid.NewGuid().ToString("N"); }

      public BaseService()
      {
          RepositoryFactory = new RepositoryFactory();
      }

      ~BaseService() 
      {
          ContextManager.Dispose();
      }
    }
}
