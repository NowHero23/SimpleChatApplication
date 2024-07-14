using DataAccessLayer.Data.Domain.Repositories.Abstract;
using DataAccessLayer.Data.Domain.Repositories.EntityFramework;
using DataAccessLayer.Etities;
using Microsoft.EntityFrameworkCore;
using SimpleChatApplication.Data.Domain;

namespace SimpleChatApplication.UnitTests
{
    [TestFixture]
    public class EFChatRepositoryTests
    {
        private IChatRepository _chatRepository;

        private AppDbContext _context;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .EnableSensitiveDataLogging()
            .Options;
            _context = new AppDbContext(options);
            _chatRepository = new EFChatRepository(_context);

        }
        [OneTimeTearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _context.ChangeTracker.Clear();
        }


        [Test, Order(1)]
        public void Create()
        {
            // Arrange
            var chat = new Chat { Name = "Test Chat", AuthorName = "Test" };

            // Act
            _chatRepository.Create(chat);
            _chatRepository.SaveChanges();

            // Assert
            Assert.True(_chatRepository.FindByCondition(c => c.Name == chat.Name && c.AuthorName == chat.AuthorName).Any());
        }

        [Test, Order(2)]
        public void FindAll()
        {
            // Arrange
            var chat = new Chat { Name = "Test Chat", AuthorName = "Test" };

            // Act
            _chatRepository.Create(chat);
            _chatRepository.SaveChanges();
            var result = _chatRepository.FindAll();

            // Assert
            Assert.True(result.Count() > 0);
        }
        [Test, Order(3)]
        public void Update()
        {
            // Arrange
            var oldChat = new Chat { Name = "Test Chat", AuthorName = "Test" };
            var newChat = new Chat { Name = "new Test Chat", AuthorName = "new Test" };

            // Act
            _chatRepository.Create(oldChat);
            _chatRepository.SaveChanges();

            var chat = _chatRepository.FindByCondition(c => c.Name == oldChat.Name && c.AuthorName == oldChat.AuthorName).AsNoTracking().FirstOrDefault();

            Assert.IsNotNull(chat);
            _context.ChangeTracker.Clear();
            if (chat != null)
            {
                chat.Name = newChat.Name;
                chat.AuthorName = newChat.AuthorName;

                _chatRepository.Update(chat);
                _chatRepository.SaveChanges();
            }

            var result = _chatRepository.FindByCondition(c => c.Id == chat.Id).AsNoTracking().FirstOrDefault();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newChat.Name, result.Name);
            Assert.AreEqual(newChat.AuthorName, result.AuthorName);
        }


        [Test, Order(4)]
        public void Remove()
        {
            // Arrange
            var newChat = new Chat { Name = "Test Chat del", AuthorName = "Test del" };

            // Act
            _chatRepository.Create(newChat);
            _chatRepository.SaveChanges();

            var chat = _chatRepository.FindByCondition(c => c.Name == newChat.Name && c.AuthorName == newChat.AuthorName).AsNoTracking().FirstOrDefault();

            Assert.NotNull(chat);
            _context.ChangeTracker.Clear();
            if (chat != null)
            {
                _chatRepository.Remove(chat);
                _chatRepository.SaveChanges();
            }

            var result = _chatRepository.FindByCondition(c => c.Name == newChat.Name && c.AuthorName == newChat.AuthorName).AsNoTracking().FirstOrDefault();

            // Assert
            Assert.IsNull(result);

        }
    }
}
