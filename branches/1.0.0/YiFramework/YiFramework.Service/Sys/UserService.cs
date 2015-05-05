using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using YiFramework.Core;
using YiFramework.DBModel;
using YiFramework.ViewModel.Sys;

namespace YiFramework.Service
{
   public class UserService:BaseService
    {
       private readonly IRepository<T_SYS_USER> _userRep;

       public UserService()
       {
           _userRep=RepositoryFactory.GetBaseRepository<T_SYS_USER>();
       }

       public UserVM GetByID(string id) 
       {
           var user = _userRep.GetByKey(id);
           if (user == null) { throw new Exception("没有找到对应用户"); }
           return Mapper.Map<T_SYS_USER,UserVM>(user);
          // return null;
       }

       public OperateResult Add(T_SYS_USER user) 
       {
           user.ID = GetPrimaryKeyValue();
           user.CreateTime = DateTime.Now;
           OperateResult result = new OperateResult();
           result.success=_userRep.Add(user, true);
           result.SetMessage("添加成功","添加失败");
           return result;
       }


    }
}
