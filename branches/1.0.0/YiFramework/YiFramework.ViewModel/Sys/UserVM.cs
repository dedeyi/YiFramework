using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.ViewModel.Sys
{
   public class UserVM
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string LoginName { get; set; }
        public DateTime CreateTime { get; set; }
       // public string DeptID { get; set; }
       // public string DeptName { get; set; }
    }
}
