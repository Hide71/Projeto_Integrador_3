using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Models;

public class UserLoginRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}