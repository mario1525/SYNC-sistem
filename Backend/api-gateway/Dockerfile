# Utilizar una imagen base oficial de .NET para el runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Utilizar una imagen base oficial de .NET SDK para la compilación
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar las dependencias
COPY ["api-gateway.csproj", "./"]
RUN dotnet restore "api-gateway.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src"
RUN dotnet build "api-gateway.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "api-gateway.csproj" -c Release -o /app/publish

# Configurar la imagen final con el runtime de ASP.NET
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 🔥 Asegurar que el archivo de configuración de Ocelot esté presente
COPY ocelot.json /app/ocelot.json

ENTRYPOINT ["dotnet", "api-gateway.dll"]
