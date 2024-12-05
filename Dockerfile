# Usamos la imagen base de SDK de .NET para construir el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establecemos el directorio de trabajo
WORKDIR /src

# Copiamos el archivo .csproj del proyecto PROYECTOPDF al contenedor
COPY ["PROYECTOPDF/proyectopdf.csproj", "PROYECTOPDF/"]

# Restauramos las dependencias del proyecto
RUN dotnet restore "PROYECTOPDF/proyectopdf.csproj"

# Copiamos el resto del código
COPY . .

# Publicamos el proyecto
RUN dotnet publish "PROYECTOPDF/proyectopdf.csproj" -c Release -o /app/publish

# Imagen final para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Establecemos el directorio de trabajo en la imagen final
WORKDIR /app

# Copiamos la publicación desde la imagen de construcción
COPY --from=build /app/publish .

# Exponemos el puerto en el que la app se ejecutará
EXPOSE 18444

# Comando para iniciar la aplicación
ENTRYPOINT ["dotnet", "PROYECTOPDF.dll"]
