using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Etities
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AuthorName { get; set; } //UserId

        public ICollection<UserConnection> UserConnections { get; set; } = new List<UserConnection>();
    }
}
