using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set;}               

        public virtual ICollection<Expense> Expenses { get; set; }
        public User User { get; set; }
    }
}
