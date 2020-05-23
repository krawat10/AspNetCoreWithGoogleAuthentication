using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lab3.Models
{
    public class Book
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsRented { get; set; }
        [JsonIgnore] public ICollection<UserBook> UserBooks { get; set; }
    }
}