using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using YiFramework.Core;
namespace YiFramework.Core
{
    /// <summary>
    /// 当前登录用户信息
    /// </summary>
    public partial class CurrentUser
    {
        private readonly string SUPERADMIN;

        public CurrentUser()
        {
            
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 当前用户部门ID
        /// </summary>
        public string DeptID { get; set; }
        /// <summary>
        /// 当前用户部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 当前用户部门类型
        /// </summary>
        public List<string> DeptType { get; set; }

        /// <summary>
        /// 当前部门的上级部门ID
        /// </summary>
        public string ParentDeptID { get; set; }
        /// <summary>
        /// 当前部门的上级部门名称
        /// </summary>
        public string ParentDeptName { get; set; }

        /// <summary>
        /// 当前用户的下属部门NameID数组
        /// </summary>
        public IList<NameID> ChildrenDept { get; set; }

        /// <summary>
        /// 当前用户的下属部门及自己当前部门
        /// </summary>
        public IList<NameID> ChildrenDeptIncludeSelf { get; set; }

        /// <summary>
        /// 当前允许访问的部门
        /// </summary>
        public IList<NameID> PermissionDetpData { get; set; }

        /// <summary>
        /// 当前允许访问的部门，包括自己所在部门
        /// </summary>
        public IList<NameID> PermissionDeptDataIncludeSelf { get; set; }

        /// <summary>
        /// 前台筛选过滤后的部门集合 (及切换部门选中的子节点)
        /// </summary>
        public IList<NameID> SelectedBrowseDept { get; set; }

        /// <summary>
        /// 是否只允许读取自己的权限数据
        /// </summary>
        public bool OnlySelfDataPermission 
        {
            get
            {
                bool iResult = false;
                var user = this;
                if (user.IsSuperAdmin) { return false; }
                if (user.PermissionDetpData == null || user.PermissionDetpData.Count() == 0)
                { iResult = true; }
                return iResult;
            }
        }

        /// <summary>
        /// 数据过滤后的部门id集合
        /// </summary>
        public List<string> DataPermissionDeptIDs
        {
            get
            {
                if (this.OnlySelfDataPermission) { return null; }
                var user = this;
                var perm = user.PermissionDetpData.Select(a => a.ID.ToString());
                var sel = user.SelectedBrowseDept.Select(a => a.ID.ToString());
                if (sel == null || sel.Count() == 0) { return null; }
                return sel.Intersect(perm).ToList();
            }
        }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin { 
            get 
            {
                return LoginName == SUPERADMIN;
            } 
        }
        /// <summary>
        /// 用户角色ID列表
        /// </summary>
        public string[] RoleIDs { get; set; }

    }
}
