using DataAccessLayer.Data.Domain.Repositories.Abstract;
using DataAccessLayer.Etities;
using SimpleChatApplication.Data.Domain;

namespace DataAccessLayer.Data.Domain.Repositories.EntityFramework
{
    public class EFChatRepository : EFRepositoryBase<Chat>, IChatRepository
    {
        public EFChatRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
