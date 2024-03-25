using System.Text.RegularExpressions;

namespace LoginAplicacion.Models
{
    public class PasswordValidator
    {
        public bool EsPasswordSegura(string password)
        {
            // Validar que la contraseña tenga al menos un número, una letra y que tenga entre 1 y 10 caracteres.
            Regex regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{1,10}$");
            return regex.IsMatch(password);
        }
    }
}
