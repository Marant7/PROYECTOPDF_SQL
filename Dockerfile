# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["PROYECTOPDF/PROYECTOPDF.csproj", "PROYECTOPDF/"]
COPY ["NegocioPDF/NegocioPDF.csproj", "NegocioPDF/"]
RUN dotnet restore "PROYECTOPDF/PROYECTOPDF.csproj"

# Copy the rest of the files
COPY . .

# Build the application
WORKDIR "/src/PROYECTOPDF"
RUN dotnet build "PROYECTOPDF.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PROYECTOPDF.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user
RUN useradd -m myappuser && chown -R myappuser:myappuser /app
USER myappuser

EXPOSE 80
ENTRYPOINT ["dotnet", "PROYECTOPDF.dll"]