using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
namespace Repos
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public ICollection<Advertisement> Ads { get; set; }
        public User()
        {
            Ads = new List<Advertisement>();
        }
    }
}
