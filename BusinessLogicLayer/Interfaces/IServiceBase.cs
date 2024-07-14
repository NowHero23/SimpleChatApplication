using BusinessLogicLayer.Common;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Interfaces
{
    public interface IServiceBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Result<T> Create(T connection);
        Result<T> Update(T connection);
        Result<bool> Remove(T connection);
    }

}
