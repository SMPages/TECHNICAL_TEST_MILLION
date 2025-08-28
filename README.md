# 🏠 RealEstate API — Technical Test (Million)

Hola, soy **Greydy Sebastián Marciales Rubio (DevSebas)**. Este repo contiene mi solución al **Technical Test Sr. Developer .NET** para **Million**. 
Implementé una API REST para gestionar propiedades inmobiliarias con un enfoque de **Arquitectura Hexagonal / Clean**, priorizando claridad, pruebas y buenas prácticas.

---

## 🚀 Stack que utilicé
- .NET 8 (compatible con .NET 5+)
- C# 12
- SQL Server
- Entity Framework Core 9
- AutoMapper 15
- nUnit (tests)
- Swagger (UI + OpenAPI)

---

## ⚙️ Requisitos previos
- SDK **.NET 8.0** (o superior)
- **SQL Server** local o remoto
- (Opcional) `sqlcmd` para ejecutar el script SQL
- Configura `src/RealEstate.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=RealEstateDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "S0perClaveSecreta_de_demo_cambiar_en_produccion",
    "Issuer": "RealEstate.Api",
    "Audience": "RealEstate.Client",
    "ExpiryMinutes": 60
  }
}
```

---

## ▶️ Cómo ejecuto la API (mi flujo)
1) Clonar el repo
```bash
git clone https://github.com/SMPages/TECHNICAL_TEST_MILLION.git
cd TECHNICAL_TEST_MILLION
```

2) Crear la base de datos (puedes elegir)
- **Opción A — Script SQL:** abrir `db/init.sql` en SSMS y ejecutar.
- **Opción B — Migraciones EF Core:**
  ```bash
  dotnet ef database update --project src/RealEstate.Infrastructure --startup-project src/RealEstate.Api
  ```

3) Levantar la API
```bash
dotnet run --project src/RealEstate.Api
```

4) Probar en Swagger  
👉 https://localhost:7002/swagger

---

## 🔐 Autenticación (cómo la resolví)
Expuse un endpoint para emitir **JWT** (demo users en memoria). Primero obtienes el token y luego llamas los endpoints protegidos.

- **POST** `/api/Auth/GenerateToken`  
  ```http
  POST https://localhost:7002/api/Auth/GenerateToken
  Content-Type: application/json

  {
    "username": "admin",
    "password": "admin123"
  }
  ```
  Usuarios de prueba: `admin/admin123`, `agent/agent123`, `viewer/viewer123`.

En Swagger, haz clic en **Authorize** (candado) y pega:  
`Bearer TU_TOKEN_AQUI`

---

## 📂 Estructura del proyecto (como lo organicé)
```
src/
 ├─ RealEstate.Api/               # Presentación (controllers, swagger, startup)
 ├─ RealEstate.Application/       # Casos de uso, puertos (interfaces), DTOs
 ├─ RealEstate.Domain/            # Entidades de dominio (sin EF, lógica pura)
 └─ RealEstate.Infrastructure/    # Persistencia EF Core, repos, AutoMapper
tests/
 └─ RealEstate.Tests/             # Pruebas unitarias (nUnit)
db/
 └─ init.sql                      # Script SQL (esquema + datos de ejemplo)
README.md
```

---

## 🔌 Endpoints que implementé

### Auth
- **POST** `/api/Auth/GenerateToken` → emite un JWT (según `Jwt:ExpiryMinutes`).

### Properties (protegidos por JWT; `ListProperties` es público)
- **POST** `/api/Properties/CreateProperty` → crea una propiedad.
- **POST** `/api/Properties/{id:int}/AddImageToProperty` → agrega imagen.
- **PUT** `/api/Properties/ChangePrice/{id:int}` → cambia precio y registra traza.
- **PUT** `/api/Properties/UpdateProperty/{id:int}` → actualiza datos básicos.
- **GET** `/api/Properties/ListProperties` *(AllowAnonymous)* → filtros + paginación.

---

## 📌 Ejemplos rápidos

**Crear propiedad**
```http
POST https://localhost:7002/api/Properties/CreateProperty
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "code": "P-003",
  "name": "Casa Campestre",
  "address": "Km 5 Vía La Calera",
  "city": "Bogotá",
  "price": 550000,
  "idOwner": 1,
  "year": 2020,
  "bedrooms": 4,
  "bathrooms": 3,
  "areaSqFt": 180,
  "description": "Hermosa casa campestre con vista a la ciudad."
}
```

**Agregar imagen**
```http
POST https://localhost:7002/api/Properties/2/AddImageToProperty
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "fileUrl": "/uploads/images/casa-norte.jpg",
  "isMain": true,
  "caption": "Fachada principal",
  "sortOrder": 1
}
```

**Cambiar precio**
```http
PUT https://localhost:7002/api/Properties/ChangePrice/2
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "newPrice": 420000,
  "notes": "Ajuste por remodelación"
}
```

**Actualizar propiedad**
```http
PUT https://localhost:7002/api/Properties/UpdateProperty/2
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "name": "Casa Norte Remodelada",
  "address": "Calle 123 #45-67",
  "city": "Bogotá",
  "year": 2021,
  "bedrooms": 4,
  "bathrooms": 3,
  "areaSqFt": 150,
  "description": "Remodelación completa en 2021."
}
```

**Listar con filtros (público)**
```http
GET https://localhost:7002/api/Properties/ListProperties?city=Bogotá&minPrice=200000&maxPrice=600000&page=1&pageSize=10
```

---

## 🧪 Pruebas (nUnit)
Ejecuto los tests con:
```bash
dotnet test
```
Incluí pruebas que validan creación/actualización, cambio de precio y listados con filtros/paginación.

---

## 🛠️ Decisiones técnicas (resumen)
- **Arquitectura Hexagonal:** dominio aislado de EF; repos en Infraestructura.
- **AutoMapper 15:** perfiles en `Infrastructure.Mapping`; registro en DI.
- **Performance:** `AsNoTracking()` en lecturas; paginación; filtros por índices.
- **Logging & errores:** `ILogger` + `try/catch` en repos y controllers.
- **Swagger:** documentación y pruebas desde `/swagger`.

---

## ✍️ Autor
**Greydy Sebastián Marciales Rubio (DevSebas)**  
📧 sebastianmarciales40@gmail.com  
🌐 https://dev-sebas.com/
