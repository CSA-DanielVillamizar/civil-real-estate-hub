using Microsoft.Extensions.Options;
using Plataforma.Application.Common;
using Plataforma.Application.Common.Interfaces;

namespace Plataforma.Infrastructure.ViabilidadAmbiental;

public sealed class ConfiguracionDatosBancariosProvider : IDatosBancariosProvider
{
    private readonly ViabilidadAmbientalOptions _options;

    public ConfiguracionDatosBancariosProvider(IOptions<ViabilidadAmbientalOptions> options)
    {
        _options = options.Value;
    }

    public DatosBancarios Obtener() => new(
        _options.Banco,
        _options.TipoCuenta,
        _options.NumeroCuenta,
        _options.TitularCuenta,
        _options.QrImageUrl);
}
