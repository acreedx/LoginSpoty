using System;
using System.Collections.Generic;

namespace LoginAplicacion.Model;

public partial class Usuario
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public short IntentosPassword { get; set; }

    public DateTime FechaDeActualizacion { get; set; }

    public short Estado { get; set; }

    public virtual ICollection<AudiUsuario> AudiUsuarios { get; set; } = new List<AudiUsuario>();
}
