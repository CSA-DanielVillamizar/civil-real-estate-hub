using MediatR;
using Plataforma.Application.Common.Interfaces;
using Plataforma.Domain.Leads.ValueObjects;
using Plataforma.Domain.Obras;
using Plataforma.Domain.Propiedades;

namespace Plataforma.Application.Obras.Commands.CrearProyectoObra;

public sealed class CrearProyectoObraCommandHandler : IRequestHandler<CrearProyectoObraCommand, CrearProyectoObraResult>
{
    private readonly IProyectoObraRepository _proyectoObraRepository;

    public CrearProyectoObraCommandHandler(IProyectoObraRepository proyectoObraRepository)
    {
        _proyectoObraRepository = proyectoObraRepository;
    }

    public async Task<CrearProyectoObraResult> Handle(CrearProyectoObraCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Crear(request.EmailCliente);
        var telefono = Telefono.Crear(request.TelefonoCliente, request.IndicativoCliente);
        var propiedadId = request.PropiedadId.HasValue ? new PropiedadId(request.PropiedadId.Value) : (PropiedadId?)null;

        var proyecto = ProyectoObra.Crear(
            request.NombreCliente, email, telefono, request.NombreProyecto, request.Descripcion, propiedadId);

        await _proyectoObraRepository.AddAsync(proyecto, cancellationToken);

        return new CrearProyectoObraResult(proyecto.Id.Value, proyecto.TokenAcceso);
    }
}
