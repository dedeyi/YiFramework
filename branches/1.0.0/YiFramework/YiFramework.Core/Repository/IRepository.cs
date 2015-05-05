using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YiFramework.Core
{
   public interface IRepository<TEntity>
    {
        /// <summary>
        /// 添加记录
        /// <param name="saveChange">是否保存当前上下文状态</param>
        /// </summary>
        /// <param name="entity">如果saveChange 是true,返回保存成功状态，否则返回true</param>
        bool Add(TEntity entity, bool saveChange);

        /// <summary>
        /// 删除多个对象
        /// </summary>
        /// <param name="items">多个实体对象</param>
        /// <param name="saveChange">是否保存</param>
        /// <returns>删除成功与否</returns>
        bool Delete(IEnumerable<TEntity> items, bool saveChange);
        /// <summary>
        /// 根据条件表达式删除多个对象
        /// </summary>
        /// <param name="whereLamb">条件表达式</param>
        /// <param name="saveChange">是否保存</param>
        /// <returns删除成功与否></returns>
        bool Delete(Expression<Func<TEntity, bool>> whereLamb, bool saveChange);
        /// <summary>
        /// 删除单个实体对象
        /// </summary>
        /// <param name="entity">要删除的对象</param>
        /// <param name="saveChange">是否保存</param>
        /// <returns></returns>
        bool Delete(TEntity entity, bool saveChange);

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetByKey(params object[] id);

       /// <summary>
       /// 根据传入实体中主键获取完整的实体
       /// </summary>
       /// <param name="entity"></param>
       /// <returns></returns>
        TEntity GetByKeys(TEntity entity);

        /// <summary>
        /// 获取单个实体对象，相当于SingleOrDefault
        /// </summary>
        /// <param name="whereLamb">条件表达式</param>
        /// <returns></returns>
        TEntity GetEntity(Expression<Func<TEntity, bool>> whereLamb);

        /// <summary>
        /// 获取所有实体集合
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetList();

        /// <summary>
        /// 获取分页集合
        /// </summary>
        /// <typeparam name="TO">排序属性的类型</typeparam>
        /// <param name="whereLamb">分页过滤条件</param>
        /// <param name="orderName">排序属性</param>
        /// <param name="isASC">是否是ASC排序</param>
        /// <param name="pagetion">分页对象</param>
        /// <returns></returns>
        IQueryable<TEntity> GetList<TO>(Expression<Func<TEntity, bool>> whereLamb, Expression<Func<TEntity, TO>> orderName, bool isASC, Pagetion pagetion);

        /// <summary>
        /// 获取总记录条数
        /// </summary>
        /// <returns></returns>
        int GetTotal();
        /// <summary>
        /// 获取指定条件记录条数
        /// </summary>
        /// <param name="whereLamb"></param>
        /// <returns></returns>
        int GetTotal(Expression<Func<TEntity, bool>> whereLamb);
        /// <summary>
        /// 保存上下文修改
        /// </summary>
        /// <returns></returns>
        int SaveChange();
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChange"></param>
        /// <returns></returns>
        bool Update(TEntity entity, bool saveChange);
        /// <summary>
        /// 查找集合
        /// </summary>
        /// <param name="whereLamb"></param>
        /// <returns></returns>
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> whereLamb);

        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        string GetKeyProperty(Type entityType);

        /// <summary>
        /// 获取多个主键
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        List<string> GetKeysProperties(Type entityType);

    }
}
