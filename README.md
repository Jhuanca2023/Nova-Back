# Neon Nova Backend

API backend para Neon Nova, desarrollada con .NET 9 siguiendo los principios de Clean Architecture.

## 📋 Estructura del Proyecto

El proyecto Neon Nova Backend sigue los principios de Clean Architecture, organizando el código en capas independientes y con responsabilidades bien definidas:

```
Neon Nova Backend
├── Domain                  # Capa de dominio - Núcleo de la aplicación
│   ├── Entities            # Clases que representan las entidades principales del negocio
│   │   └── Product.cs      # Entidad de producto con propiedades Id, Name y Price
│   └── Interfaces          # Contratos para repositorios y servicios
│       └── IProductRepository.cs # Define operaciones CRUD para productos
│
├── Application             # Capa de aplicación - Implementa la lógica de negocio
│   ├── DTOs                # Objetos para transferencia de datos entre capas
│   │   └── ProductsDTOs    # DTOs específicos para operaciones con productos
│   │       ├── CreateProductDto.cs # Para creación de productos
│   │       ├── ProductDto.cs       # Para consulta de productos
│   │       └── UpdateProductDto.cs # Para actualización de productos
│   ├── Interfaces          # Contratos para servicios de aplicación
│   │   └── IProductService.cs # Define operaciones de alto nivel para productos
│   ├── Mappings            # Configuración de mapeos entre entidades y DTOs
│   │   └── ProductMapper.cs # Mapeo entre entidades de Product y sus DTOs
│   └── Services            # Implementación de la lógica de negocio
│       └── ProductService.cs # Implementa IProductService usando el repositorio
│
├── Infrastructure          # Capa de infraestructura - Implementaciones técnicas
│   ├── Data                # Acceso a datos y persistencia
│   │   └── ApplicationDbContext.cs # Contexto de EF Core para la base de datos
│   ├── Migrations          # Migraciones de Entity Framework para la base de datos
│   └── Repositories        # Implementaciones concretas de los repositorios
│       └── ProductRepository.cs # Implementación de IProductRepository con EF Core
│
└── NeonNovaApp             # Capa de presentación - API REST
    ├── Controllers         # Controladores para exponer endpoints de la API
    │   └── ProductsController.cs # Endpoints REST para operaciones con productos
    ├── Program.cs          # Punto de entrada y configuración de la aplicación
    └── Properties          # Configuraciones de lanzamiento y despliegue
        └── launchSettings.json # Configuración para diferentes entornos
```

### Explicación del flujo de datos:

1. **Flujo de solicitud HTTP**:
   - Las peticiones llegan a los controladores en `NeonNovaApp/Controllers`
   - Los controladores inyectan e invocan servicios de la capa de aplicación

2. **Flujo de lógica de negocio**:
   - Los servicios en `Application/Services` utilizan DTOs para comunicarse con la API
   - Implementan reglas de negocio y orquestan operaciones usando repositorios
   - Utilizan `AutoMapper` para transformar entre DTOs y entidades de dominio

3. **Flujo de persistencia**:
   - Los repositorios en `Infrastructure/Repositories` implementan el acceso a datos
   - Utilizan Entity Framework Core a través de `ApplicationDbContext`
   - Transforman entidades del dominio en registros de la base de datos

## 🚀 Tecnologías

- .NET 9
- Entity Framework Core 9
- AutoMapper
- Docker
- GitHub Actions (CI/CD)
- Swagger/OpenAPI

## ⚙️ Instalación y Configuración

### Prerrequisitos

- .NET 9 SDK
- SQL Server (o Docker para contenedores)
- Visual Studio 2022+ o VS Code

### Configuración Local

1. Clonar el repositorio:
   ```
   git clone https://github.com/yourusername/neon-nova-backend.git
   cd neon-nova-backend
   ```

2. Restaurar dependencias:
   ```
   dotnet restore
   ```

3. Aplicar migraciones a la base de datos:
   ```
   dotnet ef database update --project Intrastructure --startup-project NeonNovaApp
   ```

4. Ejecutar la aplicación:
   ```
   dotnet run --project NeonNovaApp
   ```

## 🐳 Docker

### Ejecutar con Docker Compose (Recomendado)

El proyecto incluye un archivo `docker-compose.yml` que configura tanto la aplicación como una base de datos SQL Server:

```bash
# Iniciar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```

## El docker-compose.yml configura:

- neon-nova-db: SQL Server 2022 en el puerto 1433
- Base de datos con credenciales: (Usuario: sa, Contraseña: SqlPassword123)
- Volumen persistente para los datos: sqldata

## Construir y ejecutar solo la aplicación

### Si prefieres gestionar la base de datos por separado:
```bash
# Construir la imagen
docker build -t neonnovaapp:latest .

# Ejecutar el contenedor
docker run -p 8080:80 -e "ConnectionStrings__DefaultConnection=Server=tu-servidor-sql;Database=NeonNovaDB;User=sa;Password=tu-password;TrustServerCertificate=True" neonnovaapp:latest
```

## Imágenes de Docker Hub
Las imágenes oficiales están disponibles en Docker Hub:

```bash
# Descargar la última versión
docker pull ${DOCKER_HUB_USERNAME}/neonnovaapp:latest

# Ejecutar desde Docker Hub
docker run -p 8080:80 ${DOCKER_HUB_USERNAME}/neonnovaapp:latest
```

## Documentación de la API
```bash
https://localhost:7256/scalar
```
## 🧪 Tests

Para ejecutar las pruebas:

```
dotnet test
```

## 📦 API Endpoints

### Productos

- `GET /api/products` - Obtener todos los productos
- `GET /api/products/{id}` - Obtener un producto por ID
- `POST /api/products` - Crear un nuevo producto
- `PUT /api/products/{id}` - Actualizar un producto existente
- `DELETE /api/products/{id}` - Eliminar un producto

## 🔄 CI/CD

El proyecto utiliza GitHub Actions para:

- [Docker Build and Push](.github/workflows/docker-build-push.yml) - Construye y publica la imagen Docker en producción
- [Development Workflow](.github/workflows/development-workflow.yml) - Valida builds y tests en ramas de desarrollo

## 📄 Licencia

Este proyecto está licenciado bajo la licencia Apache 2.0 - consulte el archivo [LICENSE](LICENSE) para más detalles.
