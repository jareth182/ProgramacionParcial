# Portal Académico - Examen Parcial

Este proyecto es un portal web interno para la gestión de cursos y matrículas universitarias, desarrollado como parte del examen parcial de Programación. El sistema permite a los estudiantes inscribirse en cursos disponibles y a los coordinadores administrar la oferta académica.

## Stack Tecnológico 📚

* **Backend:** ASP.NET Core MVC (.NET 8)
* **Autenticación:** ASP.NET Core Identity
* **Base de Datos:** EF Core con SQLite
* **Sesión y Caché:** Redis
* **Hosting:** Render.com

---

## Cómo Ejecutar Localmente 💻

Sigue estos pasos para levantar el proyecto en tu máquina.

1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/jareth182/ProgramacionParcial.git](https://github.com/jareth182/ProgramacionParcial.git)
    ```
2.  **Navegar a la carpeta del proyecto:**
    ```bash
    cd ProgramacionParcial/Examen_parcial_2
    ```
3.  **Restaurar dependencias de .NET:**
    ```bash
    dotnet restore
    ```
4.  **Aplicar migraciones de la base de datos:**
    Este comando creará el archivo `portal.db` con las tablas y los datos iniciales necesarios.
    ```bash
    dotnet ef database update
    ```
5.  **Ejecutar la aplicación:**
    ```bash
    dotnet run
    ```
La aplicación estará disponible en `https://localhost:XXXX` y `http://localhost:YYYY`.

---
Las credenciales de inicio de sesion son coordinador@test.com , contraseña=Coordinador1*
## Variables de Entorno Requeridas ⚙️

Para que el proyecto funcione correctamente en un entorno de producción como Render, se necesitan las siguientes variables de entorno:

| Variable                            | Descripción                                                 | Ejemplo                                                         |
| ----------------------------------- | ----------------------------------------------------------- | --------------------------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT`            | Define el entorno de la aplicación.                         | `Production`                                                    |
| `ASPNETCORE_URLS`                   | Configura la URL y el puerto que usará el servidor.         | `http://0.0.0.0:${PORT}`                                        |
| `ConnectionStrings__DefaultConnection` | Cadena de conexión para la base de datos SQLite.          | `Data Source=portal.db`                                         |
| `Redis__ConnectionString`             | Cadena de conexión para la base de datos de Redis en la nube. | `redis-15979.c82.us-east-1-2.ec2.redns.redis-cloud.com:15979,password=XXX` |

---

## URL de la Aplicación Desplegada 🚀

La versión final de la aplicación está desplegada en Render y se puede acceder a través del siguiente enlace:

****

---

## URL de la Aplicación Desplegada

La aplicación está disponible en la siguiente URL:
