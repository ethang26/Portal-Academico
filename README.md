# Portal Académico

**Portal Académico** es una aplicación web que permite gestionar cursos, matriculaciones, y sesiones de usuario. El proyecto está basado en **ASP.NET Core**, utilizando **Entity Framework** para la base de datos y **Redis** para la gestión de sesiones.

## Características

- **Gestión de Cursos**: Los administradores pueden crear, editar y eliminar cursos.
- **Gestión de Matriculaciones**: Los estudiantes pueden inscribirse y cancelar su inscripción a cursos.
- **Sistema de Sesiones**: Uso de **Redis** para gestionar sesiones de usuarios.
- **Autenticación**: Integración con **ASP.NET Identity** para el registro y autenticación de usuarios.

## Tecnologías utilizadas

- **Backend**: ASP.NET Core 7.0
- **Base de Datos**: SQLite / PostgreSQL
- **Caché**: Redis
- **Autenticación**: ASP.NET Identity
- **Docker**: Para empaquetar y desplegar la aplicación (opcional)

## Requisitos previos

- **.NET SDK 7.0** o superior.
- **Docker** (si se usa Docker para el despliegue).
- **Redis** (para la gestión de sesiones).
- **SQLite** (o PostgreSQL si prefieres usar otro motor de base de datos).

## Instalación

Sigue estos pasos para configurar el proyecto en tu máquina local:

## **Clonar el repositorio**

```bash
git clone https://github.com/usuario/portal-academico.git
cd portal-academico
