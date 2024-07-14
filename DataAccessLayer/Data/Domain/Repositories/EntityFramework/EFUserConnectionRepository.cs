using DataAccessLayer.Data.Domain.Repositories.Abstract;
using DataAccessLayer.Etities;
using SimpleChatApplication.Data.Domain;

namespace DataAccessLayer.Data.Domain.Repositories.EntityFramework
{
    public class EFUserConnectionRepository : EFRepositoryBase<UserConnection>, IUserConnectionRepository
    {
        public EFUserConnectionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        /*public async Task<UserConnection> CreateAsync(UserConnection entity)
        {
            await _appDbContext.UserConnections.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(UserConnection entity)
        {
            var chat = await _appDbContext.Chats.FindAsync(entity);
            if (chat == null) return false;

            _appDbContext.Chats.Remove(chat);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public IQueryable<UserConnection> FindByConditions(Expression<Func<UserConnection, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserConnection>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserConnection> UpdateAsync(UserConnection entity)
        {
            throw new NotImplementedException();
        }*/
    }
}
