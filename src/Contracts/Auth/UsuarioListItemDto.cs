namespace Plataforma.Contracts.Auth;

public sealed record UsuarioListItemDto(Guid Id, string Nombre, string Email, string Rol, bool Activo, DateTimeOffset CreadoEn);
