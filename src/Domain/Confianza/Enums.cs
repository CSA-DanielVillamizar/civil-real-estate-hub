namespace Plataforma.Domain.Confianza;

// Un mismo agregado sirve dos vitrinas de contenido de confianza: testimonios
// de clientes (Titulo=nombre del cliente, Descripcion=la cita) y casos de
// portafolio (Titulo=nombre del proyecto, Descripcion=el resumen del caso).
// Se unifican en un solo aggregate porque ambos comparten el mismo ciclo de
// vida editorial (crear en borrador → publicar/despublicar) y los mismos
// campos de contexto (servicio relacionado, municipio) — separarlos en dos
// aggregates hubiera duplicado toda la infraestructura CRUD sin ganar nada
// en reglas de negocio distintas.
public enum TipoContenidoConfianza
{
    Testimonio,
    Portafolio
}
