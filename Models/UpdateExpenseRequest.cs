using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Models;

public class UpdateExpenseRequest
{
    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public int UserId { get; set;}

    [Required]
    public int CategoryId { get; set;}   
}

