namespace Plataforma.Application.Common.Messaging;

// Contrato serializable entre el productor (LeadCaptadoEventHandler, que
// encola) y el consumidor (Infrastructure: el BackgroundService que procesa
// la cola). Deliberadamente mínimo — solo el Id; el consumidor vuelve a leer
// el Lead desde el repositorio para tener siempre el dato más reciente,
// en vez de arrastrar una copia potencialmente obsoleta en el mensaje.
public sealed record LeadCaptadoNotificationMessage(Guid LeadId);
