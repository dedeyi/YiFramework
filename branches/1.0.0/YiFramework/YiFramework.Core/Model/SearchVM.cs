using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.Core
{
    /// <summary>
    /// 高级搜索，基模型
    /// </summary>
   public abstract class SearchVM
    {
        /// <summary>
        /// 高级搜索是否有值
        /// </summary>
        public bool HasValue { get; set; }
    }
}
