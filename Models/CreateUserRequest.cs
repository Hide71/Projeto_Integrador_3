using System;
using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Models;

public class CreateUserRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Url { get; set; }
}

