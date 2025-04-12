using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Models;

public class CreateExpenseRequest
{
    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public int CategoryId { get; set;} 
    
    [Required]
    public int AccountId { get; set; }

  
}

