using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.Core
{
    /// <summary>
    /// 数据权限验证字段
    /// </summary>
    public class DataCheckField
    {
        public DataCheckField()
        {
            this.CreatorID = "CreatorID";
            this.CreateDeptID = "CreateDeptID";
        }

        public DataCheckField(string creatorID,string createDeptID)
        {
            this.CreatorID = creatorID;
            this.CreateDeptID = createDeptID;
        }

        public string CreatorID { get; set; }
        public string CreateDeptID { get; set; }
    }
}
