# Utilizar una imagen base oficial de .NET para el runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Utilizar una imagen base oficial de .NET SDK para la compilación
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar las dependencias
COPY ["Controllers/Controllers.csproj", "Controllers/"]
RUN dotnet restore "Controllers/Controllers.csproj"

# Copiar el resto de los archivos y compilar la aplicación
COPY . .
WORKDIR "/src/Controllers"
RUN dotnet build "Controllers.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "Controllers.csproj" -c Release -o /app/publish

# Configurar el contenedor final con el runtime de ASP.NET
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ["Controllers/appsettings.json", "/app/appsettings.json"]

ENTRYPOINT ["dotnet", "Controllers.dll"]
