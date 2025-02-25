using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public TypeAccount TypeAccount {get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
     public enum TypeAccount {corrente, poupanca, carteira }
}
