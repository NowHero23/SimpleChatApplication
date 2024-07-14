using DataAccessLayer.Data.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using SimpleChatApplication.Data.Domain;
using System.Linq.Expressions;

namespace DataAccessLayer.Data.Domain.Repositories.EntityFramework
{
    public abstract class EFRepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected AppDbContext _appDbContext { get; set; }
        public EFRepositoryBase(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void Create(T entity) => _appDbContext.Set<T>().Add(entity);

        public void Remove(T entity) => _appDbContext.Set<T>().Remove(entity);

        public IQueryable<T> FindAll() => _appDbContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            _appDbContext.Set<T>().Where(expression).AsNoTracking();

        public void Update(T entity) => _appDbContext.Set<T>().Update(entity);

        public void SaveChanges() => _appDbContext.SaveChanges();
    }
}
