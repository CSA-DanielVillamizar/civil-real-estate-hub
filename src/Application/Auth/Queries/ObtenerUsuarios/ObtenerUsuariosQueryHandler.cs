using MediatR;
using Plataforma.Application.Common.Interfaces;

namespace Plataforma.Application.Auth.Queries.ObtenerUsuarios;

public sealed class ObtenerUsuariosQueryHandler : IRequestHandler<ObtenerUsuariosQuery, IReadOnlyList<UsuarioListItem>>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ObtenerUsuariosQueryHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IReadOnlyList<UsuarioListItem>> Handle(ObtenerUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioRepository.ListAsync(cancellationToken);

        return usuarios
            .Select(u => new UsuarioListItem(u.Id.Value, u.Nombre, u.Email.Valor, u.Rol.ToString(), u.Activo, u.CreadoEn))
            .ToList();
    }
}
