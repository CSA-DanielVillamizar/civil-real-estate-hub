# Contracts — Budgets

`POST /api/budgets/calculate` reutiliza directamente los tipos compartidos de `Plataforma.Contracts.Common`:

- Request: `DatosCalculoObraDto`
- Validator: `DatosCalculoObraDtoValidator`
- Response: `EstimacionCostoDto`

No se define un DTO propio de "Budgets" porque el request/response es idéntico al usado dentro de
`CreateLeadRequest.DatosCalculoObra` (ver `src/Contracts/Leads`) — esto evita duplicar la forma del
contrato entre el endpoint standalone de la calculadora y el flujo de captación de leads.
