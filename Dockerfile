# Etapa 1: Construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establecer el directorio de trabajo en el contenedor
WORKDIR /src

# Copiar el archivo .csproj del proyecto PROYECTOPDF al contenedor
COPY ["PROYECTOPDF/proyectopdf.csproj", "PROYECTOPDF/"]

# Restaurar las dependencias del proyecto
RUN dotnet restore "PROYECTOPDF/proyectopdf.csproj"

# Copiar el resto del código fuente al contenedor
COPY . .

# Construir el proyecto
RUN dotnet build "PROYECTOPDF/proyectopdf.csproj" -c Release -o /app/build

# Etapa 2: Publicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Establecer el directorio de trabajo para la aplicación
WORKDIR /app

# Copiar los archivos generados en la etapa anterior
COPY --from=build /app/build .

# Exponer el puerto en el que se ejecutará la aplicación
EXPOSE 80

# Establecer el punto de entrada para la aplicación
ENTRYPOINT ["dotnet", "PROYECTOPDF.dll"]