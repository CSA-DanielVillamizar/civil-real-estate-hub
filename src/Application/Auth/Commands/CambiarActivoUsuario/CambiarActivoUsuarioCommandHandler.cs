using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Usuarios;

namespace Plataforma.Application.Auth.Commands.CambiarActivoUsuario;

public sealed class CambiarActivoUsuarioCommandHandler : IRequestHandler<CambiarActivoUsuarioCommand, CambiarActivoUsuarioResult?>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CambiarActivoUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<CambiarActivoUsuarioResult?> Handle(CambiarActivoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(new UsuarioId(request.UsuarioId), cancellationToken);
        if (usuario is null)
            return null;

        if (request.Activo)
            usuario.Activar();
        else
            usuario.Desactivar();

        await _usuarioRepository.UpdateAsync(usuario, cancellationToken);

        return new CambiarActivoUsuarioResult(usuario.Id.Value, usuario.Activo);
    }
}
