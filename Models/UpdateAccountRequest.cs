using System.ComponentModel.DataAnnotations;
using Controle_Pessoal.Entities;

namespace Controle_Pessoal.Models;

public class UpdateAccountRequest
{
    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Balance { get; set; }

    [Required]
    public TypeAccount TypeAccount { get; set; }
}