# 📦 InventoryMan - Sistema de Gestión de Inventario

InventoryMan es un sistema de gestión de inventario desarrollado con .NET 8, siguiendo los principios de Clean Architecture y CQRS, utilizando PostgreSQL como base de datos.

## 📋 Tabla de Contenidos
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Despliegue](#-instalación-y-despliegue)
- [Documentación de API](#-documentación-de-api)
- [Arquitectura](#-arquitectura)
- [Decisiones Técnicas (ADR)](#-architecture-decision-record-adr)

## 🔧 Requisitos Previos

- .NET 8 SDK
- PostgreSQL 15 o superior
- Visual Studio 2022
- Docker

## 💻 Instalación y Despliegue

### Ambiente de Desarrollo Local

#### Prerequisitos
- Visual Studio 2022
- .NET 8 SDK
- PostgreSQL 15 o superior

#### Pasos de Instalación

1. Clonar el repositorio
```bash
git clone https://github.com/ravalosv/InventoryMan
cd InventoryMan
```

2. Configurar PostgreSQL

- Instalar [PostgreSQL](https://www.postgresql.org/download/)
- Crear una base de datos llamada `InventoryMan`

3. Configuración en Visual Studio 2022
- Abrir la solución `InventoryMan.sln`
- Esperar a que se restauren los paquetes NuGet automáticamente
- Alternativamente, ejecutar manualmente la restauración de dependencias:
```bash
dotnet restore
```

4. Aplicar Migraciones
- Dentro del proyecto InventoryMan.API, actualizar `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=inventoryman;Username=tuusuario;Password=tupassword"
  }
}
```
> **Nota**: Reemplazar `host`, `database`, `port`, `user` y `password` con los valores de la base de datos creada 
> 
- Abrir Package Manager Console (View -> Other Windows -> Package Manager Console)
- Seleccionar `InventoryMan.Infrastructure` como Default project 
- Ejecutar:
```bash
Update-Database
```
> **Nota**: Este comando creará las tablas necesarias en la base de datos e insertará datos de prueba.

5. Configurar Variables de Entorno
- Ubicar el archivo `launchSettings.json` en el proyecto InventoryMan.API
- Actualizar la configuración HTTPS:
```json
{
  "https": {
    "commandName": "Project",
    "launchUrl": "swagger",
    "applicationUrl": "http://localhost:8080",
    "environmentVariables": {
      "ASPNETCORE_ENVIRONMENT": "Development",
      "DATABASE_CONNECTION_STRING": "Host=host;Database=inventoryMan;Username=tuusuario;Password=tupassword"
    },
    "dotnetRunMessages": true
  }
}
```
> **Nota**: Reemplazar `host`, `database`, `port`, `user` y `password` con los valores de la base de datos creada 

6. Ejecutar el Proyecto
- Presionar F5 en Visual Studio 2022
- O usar el botón de "Play" (▶️) con el perfil "https"

> **✨ Resultado Esperado**: Se abrirá automáticamente el navegador mostrando la interfaz de Swagger con la documentación completa de las APIs.

#### Verificación de la Instalación

1. Confirmar que Swagger se carga correctamente en `http://localhost:8080/swagger`
2. Verificar que la base de datos contiene las tablas y datos iniciales
3. Probar endpoint `GET /api/tests/dbtest`
> **✨ Resultado Esperado**: Swagger debe cargar correctamente y el endpoint de prueba debe retornar 200 OK y un mensaje "Database connection successfully established"

#### Solución de Problemas Comunes

- **Error de Conexión a BD**: Verificar credenciales en `DATABASE_CONNECTION_STRING`
- **Error en Migraciones**: Asegurarse de que el proyecto Infrastructure está seleccionado en Package Manager Console
- **Swagger no Carga**: Verificar que `ASPNETCORE_ENVIRONMENT` está configurado como "Development"

### Despliegue en DigitalOcean

#### Prerequisitos
- Cuenta en DigitalOcean
- [doctl](https://docs.digitalocean.com/reference/doctl/how-to/install/) instalado y configurado
- Docker instalado localmente

#### Preparación del Contenedor

El proyecto ya incluye:
- Dockerfile en InventoryMan.API
- .dockerignore en la carpeta de la solución

#### Configuración en DigitalOcean

1. Configurar doctl
```bash
doctl auth init
```
> **✨ Resultado Esperado**: Deberás ver un mensaje de autenticación exitosa

2. Crear base de datos PostgreSQL en DO
- Ir a Databases → Create Database
- Seleccionar PostgreSQL
- Elegir el plan y región
- Guardar las credenciales de conexión

#### Configuración de la Base de Datos

Hay dos opciones para configurar la base de datos:

**Opción 1: Desde el entorno de desarrollo**
1. Ingresar a la consola de PostgreSQL y crear la base de datos `InventoryMan`
2. Modificar la cadena de conexión en `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=host;Database=InventoryMan;Port=25060;Username=user;Password=password"
  }
}
```
> **Nota**: Reemplazar `host`, `database`, `port`, `user` y `password` con los valores de la base de datos creada en DO

3. Aplicar las migraciones con `Update-Database` en Visual Studio
> **✨ Resultado Esperado**: Las tablas deberían crearse sin errores en la base de datos

**Opción 2: Mediante scripts SQL**
- Ubicación: Carpeta `Database_Scripts` en InventoryMan.Infrastructure
- Ejecutar en orden:
  1. `01_CreateDatabase.sql`: Crea la base de datos y tablas
  2. `02_Sample_Data.sql`: (Opcional) Datos de prueba
> **Nota**: Solo ejecutar el script `02_Sample_Data.sql.sql` si se desea información de prueba

#### Connection Pool en DO

1. Configuración
- Ir a Databases → Tu Base de Datos → Connection Pools → Create Connection Pool
- Name: inventoryman_pool
- Select Database: InventoryMan
- Pool Mode: Transaction
- Pool Size: 22 (recomendado inicial)
- Guardar credenciales del pool
> **Nota**: El uso del Connection Pool es opcional pero altamente recomendado para producción

#### Configuración del Entorno de Desarrollo

1. Actualizar `launchSettings.json` en InventoryMan.API:
```json
{
  "https": {
    "commandName": "Project",
    "launchUrl": "swagger",
    "applicationUrl": "http://localhost:8080",
    "environmentVariables": {
      "ASPNETCORE_ENVIRONMENT": "Development",
      "DATABASE_CONNECTION_STRING": "Host=localhost;Database=inventoryMan;Username=tuusuario;Password=tupassword"
    },
    "dotnetRunMessages": true
  }
}
```
> **Nota**: Reemplazar los valores de conexión con los de tu base de datos o connection pool en DO

2. Verificación Local
- Ejecutar con F5 en Visual Studio 2022 o botón "Play" (perfil "https")
- Verificar Swagger en `http://localhost:8080/swagger`
- Probar endpoint `GET /api/tests/dbtest`
> **✨ Resultado Esperado**: Swagger debe cargar correctamente y el endpoint de prueba debe retornar 200 OK y un mensaje "Database connection successfully established"

#### Despliegue en Container Registry

1. Crear Container Registry en DO
- Ir a Container Registry
- Seguir el asistente de creación

2. Publicar Imagen
```bash
# Asegurarse que Docker Desktop esté ejecutándose
# Desde PowerShell en la ruta de la solución:

# Construir imagen
docker-compose build

# Etiquetar imagen
docker tag inventoryman-api:latest [ruta Container Registry]:[tag de versión]

# Publicar al registry
docker push [ruta Container Registry]:[tag de versión]
```
> **Nota**: Ajustar los nombres de repositorios según tu cuenta de DigitalOcean

3. Verificación de Publicación
- En DO → Container Registry → Repositories
- Buscar `inventoryman-api` y verificar el tag
> **✨ Resultado Esperado**: Debe encontrarse el repositorio inventoryman-api con el tag de versión proporcionado

#### Crear App en DigitalOcean

1. Configuración Básica
- App Platform → Create App
- Seleccionar "Container Registry"
- Elegir imagen y tag
- Nombrar: "inventoryman-app"

2. Variables de Entorno
```
DATABASE_CONNECTION_STRING=Host=host;Port=25060;Database=inventoryman;Username=user;Password=password
PORT=8080
ASPNETCORE_URLS=http://+:${PORT}
ASPNETCORE_ENVIRONMENT=Production
```
> **Nota**: Usar la cadena de conexión del connection pool si se configuró

3. Configuración Adicional
- Ports: 8080
- Health Check:
  ```
  HTTP Path: /api/tests/health
  Initial Delay: 20
  Period: 30
  Timeout: 10
  Success Threshold: 1
  Failure Threshold: 3
  ```

#### Comandos Útiles

```bash
# Gestión de Apps
doctl apps logs <app-id>
doctl apps list
doctl apps create-deployment <app-id>

# Gestión del Registry
doctl registry repository list
doctl registry repository list-tags [NOMBRE-REPO]
doctl registry repository delete-tag [NOMBRE-REPO] [TAG]
```

#### Verificación Final

1. Probar el endpoint de salud:
```
https://[UrlServer]/api/tests/dbtest
```
> **✨ Resultado Esperado**: El endpoint debe devolver un código 200 y un mensaje "Database connection successfully established"

### Notas
- Ajustar los nombres de repositorios y tags según tu cuenta de DigitalOcean

## 📚 Documentación de API

La documentación API se puede revisar dentro de swagger en la ruta
```
https://[UrlServer]/swagger
```

## 🏗️ Arquitectura

El proyecto sigue los principios de Clean Architecture con las siguientes capas:

- **Core**: Entidades y reglas de negocio
- **Application**: Casos de uso y lógica de aplicación
- **Infrastructure**: Implementaciones técnicas y acceso a datos
- **API**: Capa de presentación y endpoints

### Diagrama de Arquitectura

```mermaid
graph TB
  %% Definición de subgrafos para cada capa
  subgraph Presentación["Capa de Presentación"]
      API["InventoryMan.API"]
      Controllers["Controllers<br/>- Inventory<br/>- Products<br/>- Stores<br/>- Tests"]
      Swagger["Swagger/OpenAPI"]
      
      API --> Controllers
      API --> Swagger
  end

  subgraph Aplicación["Capa de Aplicación (InventoryMan.Application)"]
      CQRS["CQRS/MediatR"]
      
      subgraph Commands["Commands"]
          IC["Inventory Commands<br/>- TransferStock<br/>- UpdateMinStock<br/>- UpdateStock"]
          PC["Product Commands<br/>- CreateProduct<br/>- DeleteProduct<br/>- UpdateProduct"]
      end
      
      subgraph Queries["Queries"]
          IQ["Inventory Queries<br/>- GetInventoryByStock<br/>- GetLowStockItems"]
          PQ["Product Queries<br/>- GetProductById<br/>- GetProducts"]
      end
      
      subgraph Behaviors["Behaviors"]
          B1["LoggingBehavior"]
          B2["PerformanceBehavior"]
          B3["ValidationBehavior"]
      end
      
      CQRS --> Commands
      CQRS --> Queries
      CQRS --> Behaviors
  end

  subgraph Core["Capa de Dominio (InventoryMan.Core)"]
      Entities["Entidades<br/>- Inventory<br/>- Movement<br/>- Product<br/>- ProductCategory<br/>- Store"]
      Interfaces["Interfaces<br/>- IInventoryRepository<br/>- IProductRepository<br/>- IStoreRepository<br/>- IUnitOfWork"]
  end

  subgraph Infraestructura["Capa de Infraestructura (InventoryMan.Infrastructure)"]
      Context["InventoryDbContext"]
      Repos["Repositories<br/>- InventoryRepository<br/>- ProductRepository<br/>- StoreRepository"]
      UoW["UnitOfWork"]
      DB[(PostgreSQL)]
      
      Context --> DB
      Repos --> Context
      UoW --> Repos
  end

  %% Conexiones entre capas
  Controllers --> CQRS
  CQRS --> Entities
  CQRS --> Interfaces
  Repos --> Interfaces
  
  %% Herramientas y frameworks externos
  Tools["Herramientas<br/>- FluentValidation<br/>- AutoMapper<br/>- Serilog"]
  
  API --> Tools
  CQRS --> Tools

  %% Estilos
  classDef presentation fill:#f9f,stroke:#333,stroke-width:2px;
  classDef application fill:#bbf,stroke:#333,stroke-width:2px;
  classDef domain fill:#bfb,stroke:#333,stroke-width:2px;
  classDef infrastructure fill:#fbb,stroke:#333,stroke-width:2px;
  classDef tools fill:#fff,stroke:#333,stroke-width:2px;

  class API,Controllers,Swagger presentation;
  class CQRS,Commands,Queries,Behaviors,IC,PC,IQ,PQ application;
  class Entities,Interfaces domain;
  class Context,Repos,UoW,DB infrastructure;
  class Tools tools;
  ```

  ## 🔨 Architecture Decision Record (ADR)

### 1. Arquitectura Base

#### Decisión
- Implementación de Clean Architecture con CQRS

#### Contexto
- Necesidad de una arquitectura escalable y mantenible para un sistema de gestión de inventario

#### Consecuencias
- **Positivas**:
  - Clara separación de responsabilidades
  - Facilita el testing
  - Independencia de frameworks
  - Código más organizado y mantenible
- **Negativas**:
  - Mayor complejidad inicial
  - Más boilerplate code

### 2. Patrón CQRS

#### Decisión
- Separación de operaciones de lectura (Queries) y escritura (Commands) usando MediatR

#### Contexto
- Necesidad de separar operaciones de lectura y escritura para mejor manejo de la lógica de negocio

#### Consecuencias
- **Positivas**:
  - Mejor organización del código
  - Facilita la implementación de nuevas características
  - Separación clara de responsabilidades
- **Negativas**:
  - Incrementa la complejidad para operaciones simples
  - Requiere más archivos y clases

### 3. Base de Datos

#### Decisión
- PostgreSQL con Entity Framework Core

#### Contexto
- Necesidad de un sistema de base de datos robusto y confiable

#### Consecuencias
- **Positivas**:
  - Base de datos relacional robusta y de código abierto
  - Buen soporte para operaciones CRUD
  - Excelente integración con EF Core
- **Negativas**:
  - Requiere mantenimiento y configuración del pool de conexiones

### 4. API REST

#### Decisión
- API REST con ASP.NET Core
- Documentación con Swagger/OpenAPI

#### Contexto
- Necesidad de exponer endpoints para gestión de inventario, productos y tiendas

#### Consecuencias
- **Positivas**:
  - API bien documentada y fácil de consumir
  - Endpoints consistentes y RESTful
  - Fácil integración con otros sistemas
- **Negativas**:
  - Overhead de documentación

### 5. Validación y Comportamientos

#### Decisión
- Implementación de FluentValidation
- Behaviors para logging y performance

#### Contexto
- Necesidad de validación robusta y monitoreo de operaciones

#### Consecuencias
- **Positivas**:
  - Validaciones consistentes y mantenibles
  - Monitoreo efectivo de performance
  - Logging centralizado
- **Negativas**:
  - Overhead en el pipeline de requests

### 6. Infraestructura y Despliegue

#### Decisión
- Despliegue en DigitalOcean App Platform

#### Contexto
- Necesidad de una plataforma de despliegue simple, económica y efectiva

#### Consecuencias
- **Positivas**:
  - Despliegue simplificado
  - Gestión automatizada de infraestructura
  - Escalabilidad según necesidad
- **Negativas**:
  - Dependencia de un proveedor específico
  - Costos asociados al servicio

### 7. Logging y Monitoreo

#### Decisión
- Implementación de Serilog para logging

#### Contexto
- Necesidad de registro y monitoreo de la aplicación

#### Consecuencias
- **Positivas**:
  - Logging estructurado
  - Fácil integración con diferentes sinks
  - Buena capacidad de búsqueda y análisis
- **Negativas**:
  - Requiere gestión de almacenamiento de logs

### 8. Seguridad

#### Decisión
- API sin autenticación (según requerimientos actuales)

#### Contexto
- Sistema interno sin requerimientos iniciales de autenticación

#### Consecuencias
- **Positivas**:
  - Simplicidad en la implementación
  - Menor overhead en requests
- **Negativas**:
  - Limitada protección de endpoints
  - Posible necesidad de implementación futura

## 🔧 Patrones Implementados

### 1. CQRS (Command Query Responsibility Segregation)
- Separación clara entre comandos y consultas
- Mejor mantenibilidad y escalabilidad

### 2. Unit of Work & Repository
- Gestión centralizada de transacciones
- Abstracción de la capa de datos

### 3. Mediator (MediatR)
- Desacoplamiento entre componentes
- Pipeline de comportamientos

## 🛠️ Tecnologías Principales

- **Framework**: .NET 8
- **ORM**: Entity Framework Core
- **Validación**: FluentValidation
- **Documentación API**: Swagger
- **Testing**: xUnit, Moq

## 💾 Manejo de Transacciones

Se implementó un sistema robusto de transacciones para garantizar la consistencia de datos:

```csharp
public async Task<Result<bool>> Handle(TransferStockCommand request, CancellationToken cancellationToken)
{
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        // Lógica de negocio
        await _unitOfWork.CommitTransactionAsync();
        return Result<bool>.Success(true);
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        return Result<bool>.Failure("Error durante la transferencia de stock");
    }
}
```
