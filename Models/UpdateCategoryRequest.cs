using System;
using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Models;
public class UpdateCategoryRequest{
    [Required]
    public string CategoryName { get; set; }
}