
namespace YiFramework.Core
{
   public class EasyuiPagetion
    {
       /// <summary>
       /// 每一页记录条数
       /// </summary>
        public int rows { get; set; }
       /// <summary>
       /// 当前页码
       /// </summary>
        public int page { get; set; }
       /// <summary>
       /// 总记录条数
       /// </summary>
        public int total { get; set; }
    }
}
