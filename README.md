# SYNC - Sistema de Información y Coordinación de Mantenimiento

# SYNC (Sistema de Información y Coordinación de Mantenimiento)

Sistema integral para la gestión de actividades de mantenimiento de equipos industriales, diseñado como una Progressive Web Application (PWA) con capacidades offline y multiplataforma.

## Características Principales

- Operación offline y sincronización automática
- Compatible con dispositivos móviles (tablets y teléfonos)
- Lectura de códigos de barra
- Gestión de actividades preventivas y correctivas
- Sistema de notificaciones en tiempo real
- Reportes detallados y seguimiento de mantenimiento

## Tecnologías Utilizadas

Frontend:

- Angular 16+
- Progressive Web App (PWA)
- Angular Material
- IndexedDB para almacenamiento offline
- Service Workers

Backend:

- .NET Core 7.0+
- Entity Framework Core
- SQL Server

Infraestructura:

- Docker y Docker Compose

## Requisitos

- Node.js 18+
- .NET Core SDK 7.0+
- Docker Desktop
- SQL Server 2019+

## Instalación

```bash
# Clonar el repositorio
git clone https://github.com/tuusuario/sync-maintenance.git

# Navegar al directorio del proyecto
cd sync-maintenance

# Iniciar con Docker Compose
docker-compose up -d
```

## Configuración del Entorno de Desarrollo

1. Frontend (Angular):

```bash
cd frontend
npm install
ng serve
```

1. Backend (.NET Core):

```bash
cd backend
dotnet restore
dotnet run
```

## Estructura del Proyecto

```
/
├── frontend/           # Aplicación Angular PWA
├── backend/            # API .NET Core
├── database/          # Scripts SQL y migraciones
├── docker/            # Configuraciones Docker
└── docs/             # Documentación adicional
```

## Contribución

1. Haz un Fork del proyecto
2. Crea una nueva rama (`git checkout -b feature/nueva-caracteristica`)
3. Realiza tus cambios y haz commit (`git commit -am 'Agrega nueva característica'`)
4. Sube los cambios a tu Fork (`git push origin feature/nueva-caracteristica`)
5. Crea un Pull Request

## Licencia

MIT License

Copyright (c) 2025 SYNC

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Contacto

Mario Beltran - [ma](mailto:mario.beltran@sync.com)
