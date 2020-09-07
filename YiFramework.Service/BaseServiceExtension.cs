using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YiFramework.Core;

namespace YiFramework.Service
{
    /// <summary>
   /// 对BaseBLL的扩展的泛型基类 封装对单表统一CRUD
   /// 重写SetRepository 设置repository属性
   /// </summary>
   public abstract class BaseService<TEntity>:BaseService
        where TEntity : EntityObject
    {
       /// <summary>
       /// 当前登录用户
       /// </summary>
       private SessionUser currentUser;

       /// <summary>
       /// 获取当前登录回话中的用户
       /// </summary>
       /// <returns></returns>
       public SessionUser CurrentUser
       {
           get
           {
               if (currentUser == null)
               {
                   currentUser = SessionManager.Get<SessionUser>(SessionManager.CurrentKey);
               }
               return currentUser;
           }
       }

       /// <summary>
       /// 公共字段
       /// </summary>
       public CommonFields BaseFields { get; set; }


       /// <summary>
       /// 是否为软删除(及逻辑删除)
       /// </summary>
       public bool IsSoftDelete { get; private set; }

   
       private Expression<Func<TEntity, bool>> softDelWhereLamb = null;

       public string TickTimeoutMsg = "当前记录已经被改变，请刷新后重试。";

       /// <summary>
       /// 仓储实例
       /// </summary>
       public IRepository<TEntity> Repository;

       public string EntityName;

       public BaseService()
       {
           BaseFields = new CommonFields();
           initSoftDelete();
           this.Repository = GetRepository();
           this.EntityName=GetEntityName();
       }

       #region 抽象方法

       /// <summary>
       /// 获取仓储实例
       /// </summary>
       /// <param name="repository"></param>
       public abstract IRepository<TEntity> GetRepository();
       /// <summary>
       /// 获取实体名称
       /// </summary>
       public abstract string GetEntityName();

       #endregion

       #region 查询

       /// <summary>
       /// 根据主键获取id
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
       public virtual TEntity GetByKey(params object[] id)
       {
           if (null == id) throw new ArgumentNullException();
           try
           {
               return Repository.GetByKey(id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       /// <summary>
       /// 获取表对象集合
       /// </summary>
       /// <returns></returns>
       public virtual IQueryable<TEntity> GetList()
       {
           if (IsSoftDelete) return Repository.Where(softDelWhereLamb);
           return Repository.GetList();
       }

       /// <summary>
       /// 获取过滤后的对象集合
       /// </summary>
       /// <param name="whereLamb">过滤表达式</param>
       /// <returns></returns>
       public virtual IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> whereLamb) 
       {
           if (IsSoftDelete) { return Repository.Where(softDelWhereLamb.And(whereLamb)); }
           return Repository.Where(whereLamb);
       }

       /// <summary>
       /// 获取分页列表
       /// </summary>
       /// <param name="pagetion"></param>
       /// <returns></returns>
       public virtual IQueryable<TEntity> GetList(Pagetion pagetion)
       {
           if (pagetion == null) throw new ArgumentNullException();
           return GetList(pagetion, false);
       }

       /// <summary>
       /// 获取分页列表，按照CreateTime asc排序
       /// </summary>
       /// <param name="pagetion"></param>
       /// <param name="orderbyAscCreateTime">按照CreateTime 顺序排列</param>
       /// <returns></returns>
       public virtual IQueryable<TEntity> GetList(Pagetion pagetion, bool orderbyAscCreateTime)
       {
          return GetList(null, pagetion, orderbyAscCreateTime);
       }

       /// <summary>
       /// 获取筛选结果后的分页列表，按照CreateTime asc排序
       /// </summary>
       /// <param name="whereLamb">过滤表达式</param>
       /// <param name="pagetion"></param>
       /// <param name="orderByAscCreateTime"></param>
       /// <returns></returns>
       public virtual IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> whereLamb, Pagetion pagetion, bool orderByAscCreateTime) 
       {
           if (pagetion == null) throw new ArgumentNullException("参数不能为空！");
           Type typeEntity = typeof(TEntity);

           IQueryable<TEntity> entities = null;
           if (whereLamb == null)
           {
               entities = GetList();
           }
           else 
           {
               entities = GetList(whereLamb);
           }

           #region 动态构建orderby
           string strOrderby = orderByAscCreateTime ? "OrderBy" : "orderbydescending";
           //创建一个参数c
           ParameterExpression param =
              Expression.Parameter(typeof(TEntity), "t");

           MethodCallExpression orderByCallExpression = null;

           if (typeEntity.GetProperty(BaseFields.CreateTime) != null)
           {
               orderByCallExpression = Expression.Call(
                  typeof(Queryable), strOrderby,
                  new Type[] { typeof(TEntity), typeof(DateTime) },
                  Expression.Constant(entities),
                  Expression.Lambda(Expression.Property
                  (param, BaseFields.CreateTime), param));
           }
           else if (typeEntity.GetProperty(BaseFields.Sort) != null)
           {
               orderByCallExpression = Expression.Call(
                  typeof(Queryable), "OrderBy",
                  new Type[] { typeof(TEntity), typeof(int) },
                  Expression.Constant(entities),
                  Expression.Lambda(Expression.Property
                  (param, BaseFields.Sort), param));
           }
           if (orderByCallExpression == null)
           {
               throw new Exception("目标表至少要有CreateTime 获取Sort作为排序字段");
           }
           #endregion

           //生成动态查询
           IQueryable<TEntity> query = entities
               .Provider.CreateQuery<TEntity>(orderByCallExpression).Skip(pagetion.rows * (pagetion.page - 1)).Take(pagetion.rows);
           pagetion.total = entities.Count();
           return query;
       }

       #endregion

       #region 添加

       /// <summary>
       /// 添加对象,对ID、CreatorName、CreateID、CreateTime、UpdateTime、Tick自动赋值
       /// </summary>
       /// <param name="entity">待添加的实体对象</param>
       /// <param name="user">回话中的用户</param>
       /// <returns></returns>
       public virtual OperateResult Add(TEntity entity)
       {
           if (entity == null) throw new ArgumentNullException();
           OperateResult result = new OperateResult();
           try
           {
               SessionUser user = CurrentUser;
               AttachCommonFieldsForAdd(entity, user);
               result.success = Repository.Add(entity, true);
               result.SetMessage("添加成功！", "添加失败！");
               return result;
           }
           catch (Exception ex)
           {
               result.success = false;
               result.message = ex.Message;
               result.Exception = ex;
               return result;
           }
           finally
           {
               //写入日志
               WriteLog("添加" + EntityName + " 操作结果为：【" + result.message + "】", true);
           }
       }

       #endregion

       #region 编辑
       /// <summary>
       /// 编辑对象，UpdateTime自动重新赋值，TimeStamp冲突检查
       /// </summary>
       /// <param name="entity"></param>
       /// <param name="user"></param>
       /// <returns></returns>
       public virtual OperateResult Edit(TEntity entity)
       {
           if (entity == null) throw new ArgumentNullException();
           OperateResult result = new OperateResult();
           try
           {
               SessionUser user = CurrentUser;
               TEntity m = Repository.GetByKeys(entity);
               if (IsSameTimeStamp(entity, m)) //如果并发冲突
               {
                   AttachFieldsForEdit(entity, m);
                   result.success = Repository.Update(m, true);
                   result.SetMessage("编辑成功！", "编辑失败");
               }
               else 
               {
                   result.success = false;
                   result.message = TickTimeoutMsg;
               }
               return result;
           }
           catch (Exception ex)
           {
               result.success = false;
               result.message = ex.Message;
               result.Exception = ex;
               return result;
           }
           finally
           {
               //写入日志
               WriteLog("编辑" + EntityName + " 操作结果为：【" + result.message + "】", true);
           }
       }

       /// <summary>
       /// 编辑对象
       /// </summary>
       /// <param name="entity"></param>
       /// <param name="editFields">需要编辑的属性数组</param>
       /// <param name="user"></param>
       /// <returns></returns>
       public virtual OperateResult Edit(TEntity entity, string[] editFields)
       {
           if (entity == null) throw new ArgumentNullException();
           OperateResult result = new OperateResult();
           try
           {
               SessionUser user = CurrentUser;
               TEntity m = Repository.GetByKeys(entity);
               if (IsSameTimeStamp(entity, m))
               {
                   AttachFieldsForEdit(entity, m, editFields);
                   result.success = Repository.Update(m, true);
                   result.SetMessage("编辑成功！", "编辑失败！");
               }
               else 
               {
                   result.success = false;
                   result.message = TickTimeoutMsg;
               }
               return result;
           }
           catch (Exception ex)
           {
               result.success = false;
               result.message = ex.Message;
               result.Exception = ex;
               return result;
           }
           finally
           {
               //写入日志
               WriteLog("编辑" + EntityName + " 操作结果为：【" + result.message + "】", true);
           }
       }

       #endregion

       #region 删除

       /// <summary>
       /// 删除对象，对象至少包括主键和时间戳字段
       /// </summary>
       /// <param name="entity">包括主键和时间戳字段的对象</param>
       /// <returns></returns>
       public virtual OperateResult Delete(TEntity entity) 
       {
           if (entity == null) throw new ArgumentNullException();
           OperateResult result = new OperateResult();
           try
           {
               var source = Repository.GetByKeys(entity);
               if (source == null) throw new Exception("根据id没有找到当前对象");
               if (IsSameTimeStamp(entity, source))
               {
                   if (IsSoftDelete)
                   {
                       entity.GetType().GetProperty(BaseFields.IsDel).SetValue(source, true);
                       result.success = Repository.SaveChange() > 0;
                   }
                   else
                   {
                       result.success = Repository.Delete(source, true);
                   }
                   result.SetMessage("删除成功!", "删除失败");
               }
               else 
               {
                   result.message = TickTimeoutMsg;
                   result.success = false;
               }
               return result;
           }
           catch (Exception ex)
           {
               result.success = false;
               result.message = ex.Message;
               result.Exception = ex;
               return result;
           }
           finally
           {
               //写入日志
               WriteLog("删除" + EntityName + " 操作结果为：【" + result.message + "】", true);
           }
       }

       /// <summary>
       /// 根据id主键删除
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
       public virtual OperateResult Delete(params object[] id)
       {
           if (id == null) throw new ArgumentNullException();
           OperateResult result = new OperateResult();
           try
           {
               var entity = Repository.GetByKey(id);
               if (entity == null) throw new Exception("根据id没有找到当前对象");
               if (IsSoftDelete)
               {
                   entity.GetType().GetProperty(BaseFields.IsDel).SetValue(entity, true);
                   result.success = Repository.SaveChange() > 0;
               }
               else
               {
                   result.success = Repository.Delete(entity, true);
               }
               result.SetMessage("删除成功!", "删除失败");
               return result;
           }
           catch (Exception ex)
           {
               result.success = false;
               result.message = ex.Message;
               result.Exception = ex;
               return result;
           }
           finally
           {
               //写入日志
               WriteLog("删除" + EntityName + " 操作结果为：【" + result.message + "】", true);
           }
       }

       /// <summary>
       /// 批量删除
       /// </summary>
       /// <param name="ids"></param>
       /// <returns></returns>
       public virtual OperateResult DeleteByIDs(string ids)
       {
           if (string.IsNullOrEmpty(ids)) throw new ArgumentNullException();
           OperateResult result = new OperateResult();
           try
           {
               string[] arrID = ids.Split(',');
               if (arrID == null || arrID.Length == 0) throw new ArgumentNullException();
               PropertyInfo isDelPro=typeof(TEntity).GetProperty(BaseFields.IsDel);
               foreach (var id in arrID)
               {
                   var item = Repository.GetByKey(id);
                   if (IsSoftDelete)
                   {
                       isDelPro.SetValue(item, true);
                   }
                   else
                   {
                       result.success = Repository.Delete(item, false);
                   }
               }
               result.success = Repository.SaveChange() > 0;
               result.SetMessage("删除记录成功", "删除记录失败");
               return result;
           }
           catch (Exception ex)
           {
               result.success = false;
               result.message = ex.Message;
               result.Exception = ex;
               return result;
           }
           finally
           {
               //写入日志
               WriteLog("删除" + EntityName + " 操作结果为：【" + result.message + "】", true);
           }
       }

       #endregion

       /// <summary>
       /// 当前上下文保存
       /// </summary>
       /// <returns></returns>
       public  int SaveChange() 
       {
           return Repository.SaveChange();
       }

       /// <summary>
       /// 检查时间戳是否相同
       /// </summary>
       /// <param name="target">检查目标，一般为前台传入的</param>
       /// <param name="source">原始对象，当前数据库对象</param>
       /// <returns></returns>
       protected bool IsSameTimeStamp(TEntity target, TEntity source) 
       {
           if (target == null || source == null) 
           {
               throw new ArgumentNullException("原始对象或者目标对象不能为空");
           }
           Type entityType = typeof(TEntity);
           PropertyInfo proInfo=entityType.GetProperty(BaseFields.TimeStamp);
           if (proInfo == null) { return true; } //如果没有时间戳字段，默认通过检查

           var tickVal = proInfo.GetValue(target);
           if (tickVal == null) { throw new Exception("在修改记录时候，需要带上时间戳值"); }
           return Util.EqualByteArray((byte[])proInfo.GetValue(target), (byte[])proInfo.GetValue(source));
       }

       /// <summary>
       /// 为添加实体，附上公共属性
       /// </summary>
       /// <param name="entity"></param>
       /// <param name="user"></param>
       public void AttachCommonFieldsForAdd(TEntity entity, SessionUser user)
       {
           Type entityType=typeof(TEntity);
           //设置主键
           PropertyInfo proKey = entityType.GetProperty(Repository.GetKeyProperty(entityType));
           if (proKey == null) { throw new Exception("主键必须为ID"); }
           //表中主键为varchar时，默认给赋值guid
           if (proKey.PropertyType.Name == "String")
           {
               proKey.SetValue(entity, GetPrimaryKeyValue());
           }
           PropertyInfo[] pis = typeof(TEntity).GetProperties();
           PropertyInfo piTemp = null;
           if (IsSoftDelete) 
           {
               piTemp = pis.SingleOrDefault(a => a.Name == BaseFields.IsDel);
               piTemp.SetValue(entity,false);
           }

           if (pis.Any(a => a.Name == BaseFields.CreatorID))
           {
               piTemp = pis.SingleOrDefault(a => a.Name == BaseFields.CreatorID);
               piTemp.SetValue(entity, user.ID);
           }
           if (pis.Any(a => a.Name == BaseFields.CreatorName))
           {
               piTemp = pis.SingleOrDefault(a => a.Name == BaseFields.CreatorName);
               piTemp.SetValue(entity, user.DisplayName);
           }
           if (pis.Any(a => a.Name == BaseFields.CreateTime))
           {
               piTemp = pis.SingleOrDefault(a => a.Name == BaseFields.CreateTime);
               piTemp.SetValue(entity, DateTime.Now);
           }
           if (pis.Any(a => a.Name == BaseFields.UpdateTime))
           {
               piTemp = pis.SingleOrDefault(a => a.Name == BaseFields.UpdateTime);
               piTemp.SetValue(entity, DateTime.Now);
           }
       }
       /// <summary>
       /// 把传递过来的对象属性复制给另一个上下文中对象属性
       /// </summary>
       /// <param name="source"></param>
       /// <param name="target"></param>
       /// <param name="fields">需要copy 的属性数组</param>
       public void AttachFieldsForEdit(TEntity source, TEntity target, string[] fields)
       {
           PropertyInfo[] pis = typeof(TEntity).GetProperties();
           foreach (var pi in pis)
           {
               if (fields.Contains(pi.Name))
               {
                   pi.SetValue(target, pi.GetValue(source));
               }
           }
       }
       /// <summary>
       /// 把传递过来的对象属性复制给另一个上下文中对象属性
       /// </summary>
       /// <param name="source"></param>
       /// <param name="target"></param>
       public void AttachFieldsForEdit(TEntity source, TEntity target)
       {
           string[] ignoreFields = BaseFields.GetFields();
           PropertyInfo[] pis = typeof(TEntity).GetProperties();
           foreach (var pi in pis)
           {
               if (!ignoreFields.Contains(pi.Name))
               {
                   pi.SetValue(target, pi.GetValue(source));
               }
               if (pi.Name == "UpdateTime")
               {
                   pi.SetValue(target, DateTime.Now);
               }
           }
       }

       //初始化逻辑删除
       private void initSoftDelete() 
       {
           var proInfos = typeof(TEntity).GetProperties();
           PropertyInfo proInfo = proInfos.SingleOrDefault(a => a.Name.Equals(BaseFields.IsDel,StringComparison.InvariantCultureIgnoreCase));
           IsSoftDelete = proInfo != null;
           if (IsSoftDelete) 
           {
               BaseFields.IsDel = proInfo.Name;
               softDelWhereLamb = createSoftDelExpr();
           }
       }

       /// <summary>
       /// 创建软删除表达式
       /// </summary>
       /// <returns></returns>
       private Expression<Func<TEntity, bool>> createSoftDelExpr() 
       {
           ParameterExpression param = Expression.Parameter(typeof(TEntity), "t");
           Expression<Func<TEntity, bool>> whereLambdaResult = null;
               var falseCon = Expression.Constant(false);
               MemberExpression m = Expression.PropertyOrField(param, BaseFields.IsDel);
               var q = Expression.Equal(m, falseCon);
               whereLambdaResult = Expression.Lambda<Func<TEntity, bool>>(q, param);
               return whereLambdaResult;
       }
    }
}
