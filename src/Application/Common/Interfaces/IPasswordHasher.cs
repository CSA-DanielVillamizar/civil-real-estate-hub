namespace Plataforma.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string passwordEnTextoPlano);

    // Además de indicar si la contraseña es correcta, señala (vía el out)
    // cuando el hash almacenado debería regenerarse con el algoritmo/costo
    // vigente (rehash-on-verify, patrón estándar de ASP.NET Core Identity).
    bool Verificar(string hashAlmacenado, string passwordEnTextoPlano, out bool requiereRehash);
}
