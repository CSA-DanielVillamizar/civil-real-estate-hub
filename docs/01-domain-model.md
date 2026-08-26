# Fase 1 — Modelo de Dominio (DDD)
## Plataforma Web Integral de Ingeniería Civil e Inmobiliaria

> Estado: **APROBADO** (con precisiones — ver historial de cambios al final)
> Metodología: Domain-Driven Design (DDD) sobre Clean Architecture (.NET 8) + React

---

## 0. Supuestos declarados (a validar)

Dado que la especificación de negocio original es de alto nivel, declaro los siguientes supuestos de dominio. **Corrígeme si alguno es incorrecto** — no se generará código sobre un supuesto no confirmado:

1. Los tres módulos (Inmobiliaria, Consultoría/Interventoría, CRM/Leads) son **Bounded Contexts independientes** que se comunican por eventos de dominio / integración, no por referencias directas a entidades de otros contextos (evita acoplamiento).
2. Un `Lead` del CRM puede referenciar una `Propiedad` de Inmobiliaria y/o un `Proyecto` de Consultoría, pero solo mediante su **Id** (referencia externa), nunca por composición del agregado.
3. La moneda de trabajo es **COP (peso colombiano)**, dado el contexto normativo (retiros ambientales, terminología "interventoría") — value object `Dinero` diseñado para soportar multi-moneda igualmente.
4. "Viabilidad constructiva" de una propiedad se deriva de reglas sobre `CaracteristicasTopograficas` + `RetiroAmbiental` (ej. pendiente máxima, distancia mínima a fuente hídrica) — motor de reglas simple en el dominio, no un servicio externo (por ahora).
5. `Interventoría` y `Consultoría` comparten el mismo bounded context (Presupuestos + Auditorías) porque operan sobre el mismo agregado `Proyecto`. Si en tu organización son procesos separados con reglas propias, este contexto se debe dividir.
6. La "Calculadora de obra" del CRM es un **Domain Service** sin persistencia propia; su resultado se guarda como snapshot (Value Object) dentro del `Lead` que la usó, para trazabilidad histórica aunque la fórmula cambie después.
7. No se modela todavía autenticación/autorización, ni multi-tenant — se asume una sola organización operando la plataforma.

---

## 1. Bounded Context: **Inmobiliaria**

Responsable de la publicación, características físicas/legales y ciclo de vida comercial de los inmuebles.

### 1.1 Aggregate Root: `Propiedad`

| Campo | Tipo | Notas |
|---|---|---|
| `Id` | `PropiedadId` (VO, Guid) | |
| `Titulo` | `string` | |
| `Descripcion` | `string` | |
| `TipoInmueble` | `TipoInmueble` (enum: `Lote`, `Casa`, `Apartamento`, `Local`, `Bodega`, `Finca`) | |
| `Precio` | `Dinero` (VO) | |
| `Estado` | `EstadoPropiedad` (enum: `Borrador`, `Publicada`, `Reservada`, `Vendida`, `Arrendada`, `Retirada`) | Transiciones controladas por métodos del agregado, no setters públicos |
| `Ubicacion` | `Ubicacion` (VO) | |
| `AreaTerreno` | `Area` (VO) | |
| `AreaConstruida` | `Area` (VO, nullable) | |
| `CaracteristicasTopograficas` | `CaracteristicasTopograficas` (VO) | |
| `RetirosAmbientales` | `IReadOnlyCollection<RetiroAmbiental>` (VO list) | Una propiedad puede tener varios retiros aplicables (río + vía, etc.) |
| `Multimedia` | `IReadOnlyCollection<ArchivoMultimedia>` (Entity interna) | Fotos, planos, renders |

**Invariantes del agregado:**
- No se puede publicar (`Publicar()`) una propiedad sin `Ubicacion`, `Precio` > 0 y al menos 1 `ArchivoMultimedia`.
- No se puede marcar `Vendida` una propiedad que no esté `Reservada` o `Publicada`.
- `RetirosAmbientales` es inmutable desde fuera del agregado — solo se modifica vía `AgregarRetiro(...)` / `RemoverRetiro(...)`.

### 1.2 Entidad interna: `ArchivoMultimedia`
`Id`, `Url`, `Tipo` (enum: `Foto`, `Plano`, `Render`, `Video`), `Orden`.

### 1.3 Value Objects

| VO | Campos | Notas |
|---|---|---|
| `Dinero` | `Monto` (decimal), `Moneda` (enum, default `COP`) | Compartido entre contextos (Shared Kernel) |
| `Ubicacion` | `Direccion`, `Municipio`, `Departamento`, `Coordenadas` | |
| `Coordenadas` | `Latitud`, `Longitud` | Validación de rango en constructor |
| `Area` | `Valor` (decimal), `UnidadMedida` (enum: `M2`, `Hectarea`) | |
| `CaracteristicasTopograficas` | `PendientePorcentaje`, `TipoSuelo` (enum), `Topografia` (enum: `Plana`, `Inclinada`, `Irregular`), `NivelFreaticoMetros` (nullable) | |
| `RetiroAmbiental` | `TipoFuente` (enum: `Rio`, `Quebrada`, `Bosque`, `ViaPrincipal`, `LineaAltaTension`), `DistanciaMinimaMetros`, `NormativaAplicable` (string, ej. POT del municipio) | |

### 1.4 Domain Events

- `PropiedadPublicadaEvent`
- `PropiedadActualizadaEvent`
- `PropiedadReservadaEvent`
- `PropiedadVendidaEvent`
- `PropiedadRetiradaEvent`
- `ViabilidadConstructivaEvaluadaEvent` (payload: `PropiedadId`, `EsViable`, `Restricciones[]`)

---

## 2. Bounded Context: **Gestión de Proyectos** (Consultoría + Interventoría)

Responsable de presupuestos de obra (Consultoría, fase pre-obra) y auditorías de interventoría (fase durante-obra), ambos anclados al mismo `ProyectoId`.

### 2.1 Aggregate Root: `Presupuesto`

| Campo | Tipo | Notas |
|---|---|---|
| `Id` | `PresupuestoId` | |
| `ProyectoId` | `ProyectoId` (referencia externa, VO) | El "Proyecto" como tal puede vivir en otro contexto o ser un concepto ligero aquí — ver supuesto #5 |
| `Items` | `IReadOnlyCollection<ItemPresupuesto>` (Entity interna) | |
| `Estado` | `EstadoPresupuesto` (enum: `Borrador`, `EnRevision`, `Aprobado`, `Rechazado`, `EnEjecucion`, `Cerrado`) | |
| `Total` | `Dinero` (calculado, no persistido como fuente de verdad — se deriva de `Items`) | |

**Invariantes:**
- `Total` siempre es la suma de `Items` — no se puede setear manualmente.
- No se pueden agregar/quitar `Items` si el estado es `Aprobado` o posterior (requiere `Reabrir()` explícito → nuevo evento).

### 2.2 Entidad interna: `ItemPresupuesto`
`Id`, `Descripcion`, `Categoria` (enum: `ManoDeObra`, `Materiales`, `Equipos`, `AdministracionYUtilidad`), `Cantidad`, `PrecioUnitario` (`Dinero`), `Subtotal` (calculado).

### 2.3 Aggregate Root: `Auditoria`

| Campo | Tipo | Notas |
|---|---|---|
| `Id` | `AuditoriaId` | |
| `ProyectoId` | `ProyectoId` | |
| `Auditor` | `string` (o `AuditorId` si se modela como entidad de usuarios en el futuro) | |
| `FechaProgramada` | `DateOnly` | |
| `FechaRealizada` | `DateOnly?` | |
| `Estado` | `EstadoAuditoria` (enum: `Programada`, `EnCurso`, `Completada`, `ConHallazgosPendientes`) | |
| `Hallazgos` | `IReadOnlyCollection<Hallazgo>` (Entity interna) | |

### 2.4 Entidad interna: `Hallazgo`
`Id`, `Descripcion`, `Severidad` (enum: `Baja`, `Media`, `Alta`, `Critica`), `EstadoCorreccion` (enum: `Abierto`, `EnCorreccion`, `Cerrado`), `FechaLimite` (nullable).

### 2.5 Value Objects
- `ProyectoId` — referencia externa (Guid tipado)
- `Dinero` — compartido (Shared Kernel, ver §1.3)

### 2.6 Domain Events

- `PresupuestoCreadoEvent`
- `PresupuestoAprobadoEvent`
- `PresupuestoExcedidoEvent` (cuando costo ejecutado real > `Total` presupuestado — requiere dato externo de ejecución, evaluar en Fase posterior)
- `AuditoriaProgramadaEvent`
- `AuditoriaCompletadaEvent`
- `HallazgoCriticoRegistradoEvent` (para notificar interesados de inmediato)

---

## 3. Bounded Context: **CRM / Leads**

Responsable de captación de leads y la calculadora de obra usada como imán de conversión.

### 3.1 Aggregate Root: `Lead`

| Campo | Tipo | Notas |
|---|---|---|
| `Id` | `LeadId` | |
| `Nombre` | `string` | |
| `Email` | `Email` (VO) | |
| `Telefono` | `Telefono` (VO) | |
| `Origen` | `OrigenLead` (enum: `CalculadoraObra`, `FormularioContacto`, `LandingPage`, `Referido`) | |
| `Estado` | `EstadoLead` (enum: `Nuevo`, `Contactado`, `Calificado`, `Convertido`, `Descartado`, `ContactoPendientePorReasignacion`) | Último valor: la propiedad de interés del lead se vendió; requiere que un asesor le ofrezca alternativas similares en la misma zona |
| `PropiedadDeInteresId` | `PropiedadId?` (referencia externa opcional) | |
| `ResultadoCalculadora` | `EstimacionCosto?` (VO, snapshot inmutable) | Se congela en el momento de la captación |

**Invariantes:**
- Transición a `Convertido` requiere que `Estado` previo sea `Calificado`.
- `ResultadoCalculadora` es inmutable una vez asignado (representa un hecho histórico).

### 3.2 Value Objects

| VO | Campos | Notas |
|---|---|---|
| `Email` | `Valor` (string) | Valida formato en constructor |
| `Telefono` | `Valor` (string), `Indicativo` | |
| `DatosCalculoObra` | `AreaConstruccionM2`, `TipoAcabado` (enum: `Basico`, `Medio`, `Alto`), `Municipio`, `TipoProyecto` (enum: `Vivienda`, `Comercial`, `Industrial`) | Entrada del usuario a la calculadora |
| `EstimacionCosto` | `MontoMinimo` (`Dinero`), `MontoMaximo` (`Dinero`), `Desglose` (lista de `(Categoria, Dinero)`), `DatosEntrada` (`DatosCalculoObra`) | Salida congelada de la calculadora |

### 3.3 Domain Service (sin estado propio)
`CalculadoraDeObraService` — recibe `DatosCalculoObra`, aplica tarifas/reglas (costo por m² según `TipoAcabado` y `Municipio`) y produce `EstimacionCosto`. No persiste nada por sí mismo; el `Lead` es quien decide guardar el resultado.

### 3.4 Domain Events

- `LeadCaptadoEvent`
- `CalculoObraRealizadoEvent` (se dispara aunque el usuario no deje datos de contacto — útil para analítica de conversión)
- `LeadCalificadoEvent`
- `LeadConvertidoEvent`
- `LeadDescartadoEvent`
- `LeadRequiereNuevaOfertaEvent` (payload: `LeadId`, `PropiedadVendidaId`, `Municipio`) — dispara la transición a `ContactoPendientePorReasignacion`; se suscribe al `PropiedadVendidaEvent` de Inmobiliaria

---

## 4. Shared Kernel

Elementos compartidos entre los tres contextos (mínimos, para evitar acoplamiento):

- `Dinero` (VO)
- Tipos de Id fuertemente tipados (`PropiedadId`, `ProyectoId`, `LeadId`, etc.) para evitar *primitive obsession* y mezclar ids entre agregados por error.

## 5. Comunicación entre contextos

- CRM → Inmobiliaria: `Lead.PropiedadDeInteresId` es una referencia débil (Guid). Si se requiere mostrar datos de la propiedad en el CRM, se resuelve vía consulta (read model / API interna), no vía join de agregados.
- Inmobiliaria → CRM: cuando se dispara `PropiedadVendidaEvent`, un *event handler* en el contexto CRM **NO** descalifica el lead. En su lugar dispara `LeadRequiereNuevaOfertaEvent`, que transiciona el `Lead` a `ContactoPendientePorReasignacion` para que un asesor le ofrezca propiedades similares en la misma zona. Un prospecto interesado sigue siendo valioso aunque la propiedad original ya no esté disponible.
- Gestión de Proyectos ↔ Inmobiliaria: sin acoplamiento directo en esta fase; `ProyectoId` es un concepto propio de Gestión de Proyectos, no necesariamente ligado 1:1 a una `Propiedad`.

---

## Historial de cambios

- **v1.1** — Aprobado por el usuario. Supuesto 5 confirmado (Consultoría + Interventoría comparten el bounded context "Gestión de Proyectos", anclado a `ProyectoId`). Corrección de negocio: `PropiedadVendidaEvent` ya no descarta leads; ahora dispara `LeadRequiereNuevaOfertaEvent` → estado `ContactoPendientePorReasignacion`.
- **v1.0** — Borrador inicial.

---

## Próximo paso

Fase 2 (Prompt 2) en curso: especificación OpenAPI 3.0 + DTOs en C# (`record` types) con FluentValidation — ver [`api/openapi.yaml`](../api/openapi.yaml) y [`src/Contracts`](../src/Contracts).
