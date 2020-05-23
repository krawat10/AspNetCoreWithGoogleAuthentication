using System.ComponentModel.DataAnnotations;

namespace Lab3.Models
{
    public class UserBook
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }
        public bool IsCurrentlyRented { get; set; }
    }
}