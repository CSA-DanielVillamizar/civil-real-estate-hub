using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Plataforma.Application.Common.Behaviours;
using Xunit;
using ApplicationValidationException = Plataforma.Application.Common.Exceptions.ValidationException;

namespace Plataforma.Application.Tests.Common.Behaviours;

public sealed record TestCommand(string Nombre) : IRequest<string>;

public sealed class ValidationBehaviourTests
{
    private readonly ValidationBehaviourFixture _fixture = new();

    [Fact]
    public async Task Handle_SinValidadoresRegistrados_LlamaAlSiguienteDelegadoYDevuelveSuResultado()
    {
        var sut = new ValidationBehaviour<TestCommand, string>(Enumerable.Empty<IValidator<TestCommand>>());
        var next = _fixture.NextDelegateQueDevuelve("resultado-ok");

        var resultado = await sut.Handle(new TestCommand("Ana"), next.Delegate, CancellationToken.None);

        resultado.Should().Be("resultado-ok");
        next.FueLlamado.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ConValidadorQuePasa_LlamaAlSiguienteDelegado()
    {
        var validator = _fixture.ValidadorQueDevuelve(new ValidationResult());
        var sut = new ValidationBehaviour<TestCommand, string>(new[] { validator.Object });
        var next = _fixture.NextDelegateQueDevuelve("resultado-ok");

        var resultado = await sut.Handle(new TestCommand("Ana"), next.Delegate, CancellationToken.None);

        resultado.Should().Be("resultado-ok");
        next.FueLlamado.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ConValidadorQueFalla_LanzaValidationExceptionYNoLlamaAlSiguienteDelegado()
    {
        var fallos = new[] { new ValidationFailure("Nombre", "El nombre es obligatorio.") };
        var validator = _fixture.ValidadorQueDevuelve(new ValidationResult(fallos));
        var sut = new ValidationBehaviour<TestCommand, string>(new[] { validator.Object });
        var next = _fixture.NextDelegateQueDevuelve("no debería alcanzarse");

        var act = () => sut.Handle(new TestCommand(""), next.Delegate, CancellationToken.None);

        var excepcion = await act.Should().ThrowAsync<ApplicationValidationException>();
        excepcion.Which.Errors.Should().ContainKey("Nombre")
            .WhoseValue.Should().Contain("El nombre es obligatorio.");
        next.FueLlamado.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ConMultiplesValidadoresQueFallan_AgregaLosErroresDeTodos()
    {
        var validator1 = _fixture.ValidadorQueDevuelve(
            new ValidationResult(new[] { new ValidationFailure("Nombre", "El nombre es obligatorio.") }));
        var validator2 = _fixture.ValidadorQueDevuelve(
            new ValidationResult(new[] { new ValidationFailure("Email", "El email no es válido.") }));
        var sut = new ValidationBehaviour<TestCommand, string>(new[] { validator1.Object, validator2.Object });
        var next = _fixture.NextDelegateQueDevuelve("no debería alcanzarse");

        var act = () => sut.Handle(new TestCommand(""), next.Delegate, CancellationToken.None);

        var excepcion = await act.Should().ThrowAsync<ApplicationValidationException>();
        excepcion.Which.Errors.Should().ContainKeys("Nombre", "Email");
    }
}

// Agrupa los stubs repetidos (mock de IValidator, delegate "next" instrumentado)
// para mantener los tests enfocados en el comportamiento, no en el armado.
internal sealed class ValidationBehaviourFixture
{
    public Mock<IValidator<TestCommand>> ValidadorQueDevuelve(ValidationResult resultado)
    {
        var mock = new Mock<IValidator<TestCommand>>();
        mock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);
        return mock;
    }

    public NextDelegateSpy<string> NextDelegateQueDevuelve(string valor) => new(valor);
}

internal sealed class NextDelegateSpy<TResponse>
{
    private readonly TResponse _valor;

    public NextDelegateSpy(TResponse valor) => _valor = valor;

    public bool FueLlamado { get; private set; }

    public RequestHandlerDelegate<TResponse> Delegate => () =>
    {
        FueLlamado = true;
        return Task.FromResult(_valor);
    };
}
