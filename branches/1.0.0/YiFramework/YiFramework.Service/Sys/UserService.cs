using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YiFramework.Core;
using YiFramework.DBModel;

namespace YiFramework.Service
{
   public class UserService:BaseService
    {
       private readonly IRepository<T_SYS_USER> _userRep;

       public UserService()
       {
           _userRep=RepositoryFactory.GetBaseRepository<T_SYS_USER>();
       }

       public AjaxReturn Add(T_SYS_USER user) 
       {
           user.ID = GetPrimaryKeyValue();
           user.CreateTime = DateTime.Now;
           AjaxReturn result = new AjaxReturn();
           result.success=_userRep.Add(user, true);
           result.SetMessage("添加成功","添加失败");
           return result;
       }

    }
}
