using DataAccessLayer.Etities;
using Microsoft.EntityFrameworkCore;

namespace SimpleChatApplication.Data.Domain
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
    }
}
