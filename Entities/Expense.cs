using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class Expense
    {
        public int id { get; set; }
        [Required]
        public string description { get; set; }
        public double amount { get; set; }
        public DateTime date { get; set; }
        public int userId { get; set;}               
        public User user { get; set; }

         public int categoryId { get; set;}               
        public Category category { get; set; }
    }
}
