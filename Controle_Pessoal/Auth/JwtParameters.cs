using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Auth;

public class JwtParameters
{
    [Required]
    public string key { get; set; }

    [Required]
    public string Issuer { get; set; }

    [Required]
    public string Audience { get; set; }
}
