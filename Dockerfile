# Establece la imagen base para la construcción de la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Establece la imagen base para la construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["proyectopdf/proyectopdf.csproj", "proyectopdf/"]
RUN dotnet restore "proyectopdf/proyectopdf.csproj"
COPY . .
WORKDIR "/src/proyectopdf"
RUN dotnet build "proyectopdf.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "proyectopdf.csproj" -c Release -o /app/publish

# Copia los archivos publicados al contenedor final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "proyectopdf.dll"]