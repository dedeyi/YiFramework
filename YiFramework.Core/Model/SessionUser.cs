namespace YiFramework.Core
{
    /// <summary>
    /// 当前登录用户信息
    /// </summary>
    public class SessionUser
    {
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

    }
}
