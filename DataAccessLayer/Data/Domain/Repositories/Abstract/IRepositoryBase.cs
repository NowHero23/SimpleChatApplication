using System.Linq.Expressions;

namespace DataAccessLayer.Data.Domain.Repositories.Abstract
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
