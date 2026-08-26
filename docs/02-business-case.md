# 🏢 Caso de Negocio: Plataforma Integral de Ingeniería Civil e Inmobiliaria
**Versión:** 1.0  
**Enfoque de Mercado:** Antioquia, Colombia (Urbano y Rural)  
**Modelo de Operación:** PropTech + ConTech (Property & Construction Technology)

---

## 1. Resumen Ejecutivo
El mercado inmobiliario tradicional en Colombia presenta una brecha crítica: los clientes e inversionistas compran terrenos o propiedades basados únicamente en variables comerciales (ubicación y precio), ignorando riesgos técnicos, topográficos y ambientales que luego generan sobrecostos o sanciones. 

Esta plataforma unifica la **gestión inmobiliaria** con la **consultoría e interventoría de ingeniería civil**, ofreciendo a los clientes un ecosistema donde pueden buscar, validar técnicamente, presupuestar y auditar la construcción de sus proyectos en un solo lugar.

## 2. Propuesta de Valor Diferenciada (El "Unfair Advantage")
A diferencia de las inmobiliarias tradicionales (que solo conectan comprador y vendedor) y las constructoras (que solo ejecutan), esta empresa ofrece **Mitigación Integral de Riesgos (Due Diligence Técnico)**:

* **Para compradores de lotes/terrenos:** Garantía de viabilidad constructiva antes de la firma de la promesa de compraventa.
* **Para desarrolladores/inversionistas:** Interventoría técnica y financiera estricta que asegura que el proyecto se mantenga dentro del presupuesto y la normativa.

## 3. Líneas de Negocio y Servicios

### 3.1. Inmobiliaria Técnica Especializada
Venta y alquiler de inmuebles con un valor añadido: cada ficha de propiedad rural o lote incluye un pre-análisis técnico.
* **Foco:** Proyectos de hospitalidad rural (eco-hoteles), fincas de recreo y parcelaciones.
* **Entregable:** Catálogo digital donde el cliente puede filtrar propiedades no solo por precio, sino por su "Viabilidad Constructiva".

### 3.2. Consultoría y Diseño Estructural
Asesoría enfocada en adaptar el diseño a la compleja topografía antioqueña sin afectar el entorno.
* **Bioarquitectura Modular:** Diseños adaptados al terreno (ej. viviendas de niveles escalonados).
* **Sistemas Constructivos Livianos:** Especificación y consultoría en *Light Gauge Steel Framing* (exoesqueletos de acero) para optimizar tiempos y reducir carga muerta en terrenos inclinados.

### 3.3. Interventoría y Presupuestos (Gestión de Proyectos)
Auditoría integral para proyectos de terceros o propios.
* Revisión y optimización de presupuestos de obra.
* Interventoría técnica para asegurar el cumplimiento de normativas estructurales y ambientales durante la ejecución.

## 4. Reglas de Negocio Clave (Business Rules)

Para garantizar la promesa de valor y evitar sanciones, la operación y la plataforma digital se rigen por las siguientes reglas inquebrantables:

| Regla | Descripción | Impacto en la Plataforma |
| :--- | :--- | :--- |
| **BR-01: Retiros Ambientales** | Todo diseño o viabilidad de lote que linde con fuentes hídricas (ríos, quebradas) debe garantizar un retiro mínimo obligatorio de 15 metros. | La plataforma alerta si el lote tiene afectación por retiro hídrico en la sección de viabilidad. |
| **BR-02: Topografía Segura** | Se establece una pendiente máxima de referencia (ej. 25%) para construcciones estándar. Terrenos con inclinación superior requerirán consultoría geotécnica especializada. | El algoritmo de viabilidad marca restricciones técnicas si el % de pendiente supera el umbral. |
| **BR-03: Pivotaje de Leads** | Si una propiedad se vende, el lead (cliente potencial) interesado no se descarta. El activo más valioso es la intención de compra. | El sistema cambia el estado del lead a "Pendiente por Reasignación" para ofrecer lotes similares en la zona. |

## 5. Estrategia de Crecimiento (Go-to-Market) y Captación
La plataforma web no es solo un brochure, es una máquina automatizada de adquisición de clientes (Lead Generation).

* **El Lead Magnet (Calculadora de Obra):** El usuario ingresa el área, el tipo de acabado y el municipio. A cambio de su correo y teléfono (Lead), el sistema genera una estimación instantánea de los costos directos e indirectos.
* **Conversión:** Una vez el lead entra al CRM interno, un asesor técnico lo contacta para ofrecer una consultoría formal o mostrarle lotes viables para el proyecto que acaba de cotizar.

## 6. Viabilidad Tecnológica y FinOps (Eficiencia de Costos)
Para asegurar que el modelo de negocio sea rentable desde el día uno, la infraestructura tecnológica se basará en un modelo de **Costo Cercano a Cero ($0)** durante la fase de MVP:

* **Arquitectura:** Clean Architecture, Domain-Driven Design (DDD).
* **Frontend:** React + Tailwind CSS (Hosteado en Azure Static Web Apps - Free Tier).
* **Backend:** .NET 8 Minimal APIs (Hosteado en Azure App Service - F1 Free).
* **Base de Datos:** Azure SQL Database (Serverless / Free Offer).
* **Automatización:** CI/CD con GitHub Actions.
