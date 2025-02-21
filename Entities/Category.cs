using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class Category
    {
        public int id { get; set; }
        [Required]
        public string categoryName { get; set; }
         public ICollection<Expense> expenses { get; set; }


    }
}
