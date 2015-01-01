using System.Data.Objects;
using System.Data.Objects.DataClasses;
using YiFramework.Core;

namespace YiFramework.DBModel
{
    /// <summary>
    /// 创建Repository工厂
    /// </summary>
   public class RepositoryFactory
    {
        /// <summary>
        /// 获取基础平台的仓储实例
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetBaseRepository<TEntity>()
            where TEntity : EntityObject
        {
            return GetRepository<YiFrameworkEntities, TEntity>();
        }

        /// <summary>
        /// 获取仓储实例
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        private IRepository<TEntity> GetRepository<TContext, TEntity>()
            where TContext : ObjectContext, new()
            where TEntity : EntityObject
        {
            return new BaseRepository<TContext, TEntity>();
        }
    }
}
