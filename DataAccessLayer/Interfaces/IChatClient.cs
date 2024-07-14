namespace DataAccessLayer.Interfaces
{
    public interface IChatClient
    {
        public Task ReciveMessage(string userName, string message);
        public Task ReciveStringList(string method, List<string> list);

    }
}
