using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Models;

public class CreateCategoryRequest
{
    [Required]
    public string CategoryName { get; set; }
}

