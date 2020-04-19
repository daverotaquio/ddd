using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TradingEngine.Infrastructure.Repositories.Interface.Base
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity obj);
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool enableTracking = false,
            params Expression<Func<TEntity, object>>[] includeProperties
        );
        void Update(TEntity obj);
        void Remove(int id);
        void Remove(TEntity obj);
        Task<bool> SaveChanges(CancellationToken cancellationToken);
    }
}