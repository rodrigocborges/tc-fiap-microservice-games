using System.ComponentModel;

namespace FIAPCloudGames.Domain.Enumerators
{
    public enum UserRole
    {
        [Description("Visitante")]
        Customer = 0,
        [Description("Administrador")]
        Admin = 1
    }
}
