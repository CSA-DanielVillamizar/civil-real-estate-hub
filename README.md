# 🏗️ Plataforma Integral de Ingeniería Civil e Inmobiliaria

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](#)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black)](#)
[![Azure](https://img.shields.io/badge/Hosted_on-Azure-0089D6?logo=microsoftazure&logoColor=white)](#)
[![Architecture: DDD](https://img.shields.io/badge/Architecture-DDD_%7C_Clean-success)](#)

Plataforma de software empresarial diseñada para unificar servicios de ingeniería civil (consultoría, interventoría y presupuestos) con el sector inmobiliario en Colombia. El sistema no solo lista propiedades, sino que audita la viabilidad constructiva (topografía, retiros ambientales) y captura prospectos (leads) a través de una calculadora interactiva de costos de obra.

## ✨ Características Principales

* **🏢 Catálogo Inmobiliario Técnico:** Fichas de propiedades con validación automatizada de viabilidad constructiva basada en inclinación del terreno y retiros hídricos (ej. normativas ambientales en Antioquia).
* **💰 Calculadora de Obra (Lead Magnet):** Motor de estimación de costos en tiempo real (por m², tipo de acabado, etc.) que actúa como embudo de conversión para captar inversionistas y constructores.
* **📊 Gestión de Leads (CRM Minimalista):** Tracking del estado de los prospectos y su propiedad de interés, preservando un snapshot inmutable de su cotización histórica.
* **🛡️ Arquitectura Zero-Trust y FinOps:** Despliegue en la nube optimizado para costos mínimos (Serverless) utilizando *Managed Identities* y RBAC, sin credenciales expuestas en código.

## 🏛️ Arquitectura del Sistema

La solución está construida bajo los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**.

### Stack Tecnológico
* **Frontend:** React + TypeScript + Vite, estilizado con Tailwind CSS.
* **Backend:** Minimal APIs en .NET 8 (C#).
* **Base de Datos:** Azure SQL Database (Serverless / Free Offer) orquestada con Entity Framework Core.
* **Comunicación Interna:** Patrón CQRS implementado con MediatR para desacoplar casos de uso y despachar Eventos de Dominio.
* **Validaciones:** FluentValidation en el backend (interceptado en el pipeline de MediatR) y espejo de validación en el cliente.

### Estructura del Proyecto

```text
📦 src
 ┣ 📂 Plataforma.Domain         # Entidades, Value Objects, Reglas de Negocio, Eventos
 ┣ 📂 Plataforma.Application    # Casos de Uso (CQRS), Interfaces (Repositorios), DTOs
 ┣ 📂 Plataforma.Infrastructure # EF Core, Configuraciones de Base de Datos, Servicios Externos
 ┣ 📂 Plataforma.WebApi         # Minimal APIs, Inyección de Dependencias, Middleware (Manejo de Errores)
 ┗ 📂 frontend                  # SPA en React (Hooks, Componentes, Servicios API)
📦 deploy
 ┗ 📂 bicep                     # Infraestructura como Código (IaC) para Azure
