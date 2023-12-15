##Backend app de CRUD estudiantes(Api de Asp.net Core)

## Api de Asp.net Core, con los endpoints para hacer CRUD de estudiantes, permite registro,login y manejo de sesiones de estudiantes , utiliza JWT.
- Uso de BD MySQL
- Uso de JWT
- Api con documentación XML
- Manejo de errores

### Módulo de Crud de estudiantes
- Crear un estudiante.
- Modificar datos de un estudiante.
- Listar estudiantes.
- Eliminar un estudiante.

### Módulo de autenticación
- Creación de un estudiante.
- Login de estudiante.
- Gestión de token autenticación de usuario.

### Levantar la aplicación en desarrollo
 - Modificar la cadena de conexion a la BD con el usuario y password de BD en appsettings.json

### Nota: esta api usa base de datos MySQL. 
1. Dependencias a instalar:  
```dotnet add package Microsoft.EntityFrameworkCore.SqlServer``` , 
```dotnet add package Pomelo.EntityFrameworkCore.MySql``` , 
```dotnet add package Microsoft.EntityFrameworkCore.Design```

2. Intalar tools entity framework: ```dotnet tool install --global dotnet-ef```

3. Crear una migracion de la BD: ```dotnet ef migrations add InitialCreate```

4. Actualizar Base de Datos: ```dotnet ef database update```

5. Para levantar el server de desarrollo ```dotnet run```

   
