using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Etities
{
    public class UserConnection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }  //UserId

        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public string ConnectingId { get; set; }
    }
    //public record UserConnection(string UserName, string ChatRoom);
}
