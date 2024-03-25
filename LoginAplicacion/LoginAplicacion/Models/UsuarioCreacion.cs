using System;
using System.Collections.Generic;

namespace LoginAplicacion.Models;

public partial class NuevoUsuarioCreacion
{
    public string NombreUsuario { get; set; } = null!;
    public string Correo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string NuevoPassword { get; set; } = null!;

    public string ConfirmarNuevoPassword { get; set; } = null!;

}
