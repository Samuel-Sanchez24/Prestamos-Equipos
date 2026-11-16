using Prestamos.Contracts.Usuarios;

namespace Prestamos.Contracts.Auth;

public class SesionResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEn { get; set; }
    public UsuarioResponse Usuario { get; set; } = new();
}