
using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
