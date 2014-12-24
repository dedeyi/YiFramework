using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.Core
{
    /// <summary>
    /// 数据访问基类，对数据上下文增删查改最基本的封装
    /// </summary>
    /// <typeparam name="TContext">数据库上下文</typeparam>
    /// <typeparam name="TEntity">数据库表对象</typeparam>
    public class BaseRepository<TContext, TEntity> : IRepository<TEntity>
        where TContext : ObjectContext, new()
        where TEntity : EntityObject
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        protected readonly TContext Context;

        /// <summary>
        /// 当前数据集对象
        /// </summary>
        protected readonly ObjectSet<TEntity> Entities;

        public BaseRepository()
        {
            Context = ContextManager.Instance<TContext>();
            Entities = this.Context.CreateObjectSet<TEntity>();
        }
        /// <summary>
        /// 保存当前上下文修改,只对当前数据访问对象所在的上下文进行SaveChange
        /// </summary>
        public int SaveChange()
        {
            return Context.SaveChanges();
        }

        #region 查询

        /// <summary>
        /// 根据主键获取一个实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetByKey(params object[] id)
        {
            var count = 0;
            List<PropertyInfo> res_Primary = new List<PropertyInfo>();
            List<EntityKeyMember> keyMemberList = new List<EntityKeyMember>();
            PropertyInfo[] properties = typeof(TEntity).GetProperties();
            foreach (PropertyInfo pI in properties)
            {
                System.Object[] attributes = pI.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute is EdmScalarPropertyAttribute)
                    {
                        if ((attribute as EdmScalarPropertyAttribute).EntityKeyProperty && !(attribute as EdmScalarPropertyAttribute).IsNullable)
                            keyMemberList.Add(new EntityKeyMember(pI.Name, id[count]));
                        count++;
                    }

                }
            }
            return Context.GetObjectByKey(new EntityKey(Context.DefaultContainerName + "." + typeof(TEntity).Name, keyMemberList)) as TEntity;
        }

        /// <summary>
        /// 获取联合主键表的实体
        /// </summary>
        /// <param name="entity">各个联合主键必须有值</param>
        /// <returns></returns>
        public TEntity GetByKeys(TEntity entity)
        {
            List<string> keys = GetKeysProperties(typeof(TEntity));
            if (keys.Count < 1) { throw new Exception("不是联合主键对象"); }
            List<KeyValuePair<string, object>> kv = new List<KeyValuePair<string, object>>();
            Type entityType = typeof(TEntity);
            foreach (var key in keys)
            {
                object val = entityType.GetProperty(key).GetValue(entity);
                kv.Add(new KeyValuePair<string, object>(key, val));
            }
            EntityKey entityKey = new EntityKey(Context.DefaultContainerName + "." + typeof(TEntity).Name, kv);
            return Context.GetObjectByKey(entityKey) as TEntity;
        }

        /// <summary>
        /// 查询单个数据实体,相当于SingleOrDefault方法
        /// </summary>
        /// <param name="whereLamb">查询条件表达式</param>
        /// <returns>返回单个实体对象。不存在会返回null，查找到多个实体会抛出异常</returns>
        public TEntity GetEntity(Expression<Func<TEntity, bool>> whereLamb)
        {
            return Entities.SingleOrDefault(whereLamb);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereLamb">查询条件表达式</param>
        /// <returns>查询结果集</returns>
        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> whereLamb)
        {
            return this.Entities.Where(whereLamb);
        }

        /// <summary>
        /// 获取所有数据集
        /// </summary>
        /// <returns>返回 数据表集合对象</returns>
        public IQueryable<TEntity> GetList()
        {
            return Entities;
        }

        /// <summary>
        /// 获取实体表总记录条数
        /// </summary>
        /// <returns></returns>
        public int GetTotal()
        {
            return Entities.Count();
        }

        /// <summary>
        /// 获取查询条件下的记录条数
        /// <param name="whereLamb">查询条件表达式</param>
        /// </summary>
        /// <returns></returns>
        public int GetTotal(Expression<Func<TEntity, bool>> whereLamb)
        {
            return Entities.Count(whereLamb);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="orderType">排序字段类型</typeparam>
        /// <param name="exp">筛选表达式</param>
        /// <param name="orderName">排序字段</param>
        /// <param name="isASC">是否ASC排序</param>
        /// <param name="pagetion"></param>
        /// <returns>排序结果集</returns>
        public IQueryable<TEntity> GetList<orderType>(Expression<Func<TEntity, bool>> whereLamb, Expression<Func<TEntity, orderType>> orderName, bool isASC, EasyuiPagetion pagetion)
        {
            pagetion.total = GetTotal(whereLamb);
            if (isASC)
            {
                return Entities
                    .Where(whereLamb)
                    .OrderBy(orderName)
                    .Skip((pagetion.page - 1) * pagetion.rows)
                    .Take(pagetion.rows)
                    .AsQueryable();
            }
            return Entities
                .Where(whereLamb)
                .OrderByDescending(orderName)
                .Skip((pagetion.page - 1) * pagetion.rows)
                .Take(pagetion.rows)
                .AsQueryable();
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加记录
        /// <param name="saveChange">是否保存当前上下文状态</param>
        /// </summary>
        /// <param name="entity">如果saveChange 是true,返回保存成功状态，否则返回true</param>
        public bool Add(TEntity entity, bool saveChange)
        {
            if (entity == null) throw new ArgumentNullException();
            Entities.AddObject(entity);
            if (saveChange) { return this.SaveChange() > 0; }
            return true;
        }


        #endregion

        #region 编辑

        /// <summary>
        /// 更新 【要修改的对象entity，必须保证存在于上下文之中(通过EF查询出来的而非自己构造的)】
        /// </summary>
        /// <param name="entity">待修改的实体对象</param>
        /// <param name="saveChange">是否保存当前上下文状态</param>
        public bool Update(TEntity entity, bool saveChange)
        {
            if (entity == null) throw new ArgumentNullException();
            if (entity.EntityKey == null) throw new Exception("更新的实体对象必须存在于上下文中，不能自己构造的！");
            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
            if (saveChange) { return this.SaveChange() > 0; }
            return true;
        }

        #endregion

        #region 删除
        /// <summary>
        /// 删除 , 此处为直接删除数据库记录【要删除的对象entity，必须保证存在于上下文之中(通过EF查询出来的而非自己构造的)】
        /// </summary>
        /// <param name="entity">待删除的实体对象</param>
        /// <param name="saveChange">是否保存当前上下文状态</param>
        public bool Delete(TEntity entity, bool saveChange)
        {
            if (entity == null) throw new ArgumentNullException();
            if (entity.EntityKey == null) throw new Exception("删除的实体对象必须存在于上下文中，不能自己构造的！");
            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);
            if (saveChange) { return this.SaveChange() > 0; }
            return true;
        }

        /// <summary>
        /// 根据条件 批量删除记录
        /// </summary>
        /// <param name="whereLamb"></param>
        /// <param name="saveChange"></param>
        /// <returns></returns>
        public bool Delete(Expression<Func<TEntity, bool>> whereLamb, bool saveChange)
        {
            if (whereLamb == null) throw new ArgumentNullException();
            var items = Entities.Where(whereLamb);
            if (items != null)
            {
                foreach (var item in items)
                {
                    Context.ObjectStateManager.ChangeObjectState(item, EntityState.Deleted);
                }
            }
            if (saveChange) { return this.SaveChange() > 0; }
            return true;

        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="items"></param>
        /// <param name="saveChange"></param>
        /// <returns></returns>
        public bool Delete(IEnumerable<TEntity> items, bool saveChange)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    Context.ObjectStateManager.ChangeObjectState(item, EntityState.Deleted);
                }
            }
            if (saveChange) { return this.SaveChange() > 0; }
            return true;
        }


        #endregion

        #region 执行T-SQL

        public ObjectResult<TEntity> ExecuteStoreQuery(string commandText, params Object[] para)
        {
            return Context.ExecuteStoreQuery<TEntity>(commandText, para);
        }

        public int ExecuteStoreCommand(string commandText, params Object[] para)
        {
            return Context.ExecuteStoreCommand(commandText, para);
        }

        #endregion


        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public string GetKeyProperty(Type entityType)
        {
            foreach (var prop in entityType.GetProperties())
            {
                var attr = prop.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false).FirstOrDefault()
                    as EdmScalarPropertyAttribute;
                if (attr != null && attr.EntityKeyProperty)
                    return prop.Name;
            }
            return null;
        }

        /// <summary>
        /// 获取多个主键
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public List<string> GetKeysProperties(Type entityType)
        {
            List<string> keys = new List<string>();
            foreach (var prop in entityType.GetProperties())
            {
                var attr = prop.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false).FirstOrDefault()
                    as EdmScalarPropertyAttribute;
                if (attr != null && attr.EntityKeyProperty)
                    keys.Add(prop.Name);
            }
            return keys;
        }

    }
}
