
using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class User
    {
        public int id { get; set; }
        [Required]
        public string username { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public ICollection<Expense> expenses { get; set; }
    }
}
