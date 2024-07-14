using DataAccessLayer.Etities;
using PresentationLayer.DTOs;
using SimpleChatApplication.IntegrationTests.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SimpleChatApplication.IntegrationTests
{
    [TestFixture]
    public class ChatControllerTests
    {
        private CustomWebApplicationFactory<Program> _factory;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _factory = new CustomWebApplicationFactory<Program>();
        }
        [SetUp]

        [OneTimeTearDown]
        public void Cleanup()
        {
            _factory.Dispose();
        }


        [Test, Order(1)]
        public async Task CreateChat()
        {
            // Arrange
            var client = _factory.CreateClient();
            var chat = new ChatDto { Name = "Test Chat", AuthorName = "Test" };

            // Act
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(chat),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Chat/create", jsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Chat>();
            Assert.AreEqual(chat.Name, result.Name);
            Assert.AreEqual(chat.AuthorName, result.AuthorName);
            Assert.True(result.Id != 0);
        }

        [Test, Order(2)]

        public async Task UpdateChat()
        {
            // Arrange
            var client = _factory.CreateClient();
            var oldChat = new ChatDto { Name = "Test Chat2", AuthorName = "Test2" };
            var newChat = new ChatDto { Name = "new Test Chat2", AuthorName = "new Test2" };

            // Act
            using StringContent createJsonContent = new(
                JsonSerializer.Serialize(oldChat),
                Encoding.UTF8,
                "application/json");

            var createResponse = await client.PostAsync("api/Chat/create", createJsonContent);

            newChat.Id = (await createResponse.Content.ReadFromJsonAsync<Chat>()).Id;

            using StringContent updateJsonContent = new(
                JsonSerializer.Serialize(newChat),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Chat/update", updateJsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<Chat>();
            Assert.AreEqual(newChat.Name, result.Name);
            Assert.AreEqual(newChat.AuthorName, result.AuthorName);
            Assert.True(result.Id != 0);
        }
        [Test, Order(2)]

        public async Task RemoveChatById()
        {
            // Arrange
            var client = _factory.CreateClient();
            var chat = new ChatDto { Name = "Remove Chat Test", AuthorName = "Test" };

            // Act
            using StringContent createJsonContent = new(
                JsonSerializer.Serialize(chat),
                Encoding.UTF8,
                "application/json");

            var createResponse = await client.PostAsync("api/Chat/create", createJsonContent);


            var Id = (await createResponse.Content.ReadFromJsonAsync<Chat>()).Id;

            using StringContent removeJsonContent = new(
                JsonSerializer.Serialize(Id),
                Encoding.UTF8,
                "application/json");

            var response = await client.DeleteAsync($"api/Chat/removeById?Id={Id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(result == "true");
        }


    }
}