using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
         public ICollection<Expense> Expenses { get; set; }


    }
}
