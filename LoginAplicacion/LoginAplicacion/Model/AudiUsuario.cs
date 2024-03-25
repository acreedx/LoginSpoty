using System;
using System.Collections.Generic;

namespace LoginAplicacion.Model;

public partial class AudiUsuario
{
    public int Id { get; set; }

    public int UsuariosIdUsuario { get; set; }

    public DateTime FecUltAct { get; set; }

    public string AccionRealizada { get; set; } = null!;

    public virtual Usuario UsuariosIdUsuarioNavigation { get; set; } = null!;
}
