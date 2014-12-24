using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.Core
{
   public class Util
    {
       /// <summary>
       /// 获取32位的guid字符串
       /// </summary>
       /// <returns></returns>
       public static string GetGUIDString()
       { return Guid.NewGuid().ToString("N"); }

    }
}
