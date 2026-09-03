using MediatR;

namespace Plataforma.Application.Auth.Queries.ObtenerUsuarios;

public sealed record ObtenerUsuariosQuery : IRequest<IReadOnlyList<UsuarioListItem>>;

public sealed record UsuarioListItem(Guid Id, string Nombre, string Email, string Rol, bool Activo, DateTimeOffset CreadoEn);
