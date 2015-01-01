using System.Runtime.Remoting.Messaging;

namespace YiFramework.Core
{
    /// <summary>
    /// 数据上下文存储对象
    /// </summary>
   public class ContextStorage
    {

        /// <summary>
        /// 增加
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
      public static void Add<T>(string key, T value) 
       {
          CallContext.SetData(key, value);
       }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>值</returns>
      public static T Get<T>(string key) 
           where T:class
       {
          return CallContext.GetData(key) as T;
       }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key">键</param>
      public static void Remove(string key) 
      {
          CallContext.FreeNamedDataSlot(key);
      }

        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
        public static void Set<T>(string key, T value) 
         {
             CallContext.SetData(key, value);
         }
    }
}
