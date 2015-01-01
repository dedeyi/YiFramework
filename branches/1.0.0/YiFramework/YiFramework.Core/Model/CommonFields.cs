using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.Core
{
    /// <summary>
    /// 实体的公共字段
    /// 这个在BaseServiceExtension时候会用到
    /// </summary>
    public class CommonFields
    {
        public string CreateTime { get; set; }

        public string CreatorID { get; set; }

        public string CreatorName { get; set; }

        public string UpdateTime { get; set; }

        public string TimeStamp { get; set; }

        public string Sort { get; set; }

        public string IsDel { get; set; }

        public string EntityKey { get; set; }

        public string EntityState { get; set; }

        public CommonFields()
        {
            this.CreateTime = "CreateTime";
            this.CreatorID = "CreatorID";
            this.CreatorName = "CreatorName";
            this.UpdateTime = "UpdateTime";
            this.TimeStamp = "TimeStamp";
            this.IsDel = "IsDel";
            this.Sort = "Sort";
            this.EntityKey = "EntityKey";
            this.EntityState = "EntityState";
        }

        public CommonFields(
            string createTime,
            string creatorID,
            string creatorName,
            string updateTime,
            string timeStamp,
            string sort,
            string isDel
            )
            : this(createTime, creatorID, creatorName, updateTime, timeStamp, sort,isDel, "EntityKey", "EntityState") 
        {
        }

        public CommonFields(
            string createTime, 
            string creatorID, 
            string creatorName, 
            string updateTime,
            string timeStamp,
            string sort,
            string isDel,
            string entityKey,
            string entityState
            ) 
        {
            this.CreateTime = createTime;
            this.CreatorID = creatorID;
            this.CreatorName = creatorName;
            this.UpdateTime = updateTime;
            this.TimeStamp = timeStamp;
            this.Sort = sort;
            this.IsDel = isDel;
            this.EntityKey = entityKey;
            this.EntityState = entityState;
        }

        /// <summary>
        /// 获取所有字段
        /// </summary>
        /// <returns></returns>
        public string[] GetFields() 
        {
            Type type = typeof(CommonFields);
            return type.GetProperties().Select(a => a.Name).ToArray();
        }

    }
}
