using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repos
{
    public class Advertisement
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public string PicLink { get; set; }
        public int Rating { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DeletionDate { get; set; }
    }
}
