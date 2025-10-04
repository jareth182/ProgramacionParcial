# Etapa 1: Compilación (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia todos los archivos del proyecto y restaura las dependencias
COPY . .
RUN dotnet restore "Examen parcial 2.csproj"

# Publica la aplicación para producción
RUN dotnet publish "Examen parcial 2.csproj" -c Release -o out

# Etapa 2: Ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia solo la aplicación publicada desde la etapa de compilación
COPY --from=build /app/out .

# Define el comando para iniciar la aplicación
ENTRYPOINT ["dotnet", "Examen parcial 2.dll"]