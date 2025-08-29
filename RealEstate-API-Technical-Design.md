# RealEstate API — Diseño técnico (resumen corto)
**Autor:** Greydy Sebastián Marciales Rubio (_DevSebas_)

> Qué construí, cómo está armado y qué validar al revisar. 

## 1) Qué es
API REST para gestionar propiedades (crear/editar, cambio de precio con traza, imágenes y listado público con filtros).
Tecnologías: **.NET 8**, **EF Core**, **AutoMapper**, **JWT**, **Swagger**.
Arquitectura: **Hexagonal/Clean** (Dominio aislado de EF/Web).

## 2) Cómo está organizado
```
src/
 ├ RealEstate.Api           # Controllers, Swagger, startup
 ├ RealEstate.Application   # Casos de uso, puertos (interfaces), DTOs
 ├ RealEstate.Domain        # Entidades y lógica de dominio (pura)
 └ RealEstate.Infrastructure# EF Core, repositorios, AutoMapper
tests/
 └ RealEstate.Tests         # Unit + Integration (nUnit)
```
Puntos clave: repos via interfaces, DTOs para IO, `AsNoTracking()` en lecturas, paginación y filtros en queries.

## 3) Seguridad y Swagger
- **JWT**: `POST /api/Auth/GenerateToken` (demo: `admin/admin123`, `agent/agent123`, `viewer/viewer123`).
- En Swagger: botón **Authorize** → `Bearer <token>`.
- Swagger UI: `/swagger` (incluye XML comments y esquema Bearer).

## 4) Endpoints principales
- **POST** `/api/Properties/CreateProperty` (JWT)
- **POST** `/api/Properties/{id}/AddImageToProperty` (JWT)
- **PUT**  `/api/Properties/ChangePrice/{id}` (JWT, registra traza)
- **PUT**  `/api/Properties/UpdateProperty/{id}` (JWT)
- **GET**  `/api/Properties/ListProperties` (público, filtros/paginación)

## 5) Cómo probar rápido
1. `dotnet run --project src/RealEstate.Api`
2. Pedir token y usar **Authorize** en Swagger.
3. Probar **Create/Update/ChangePrice/AddImage** y **ListProperties**.

## 6) Pruebas
- nUnit con **EF InMemory** y `WebApplicationFactory` para integración.
- Estado actual: **13/13 pruebas aprobadas**.


