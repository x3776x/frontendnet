using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class Login {
    [Required (ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress (ErrorMessage = "El campo {0} no es correo v�lido.")]
    [Display (Name = "Correo electr�nico")]
    public required string Email {
        get; set;
    }

    [Required (ErrorMessage = "El campo {0} es obligatorio.")]
    [DataType (DataType.Password)]
    [Display (Name = "Contrase�a")]
    public required string Password {
        get; set;
    }
}