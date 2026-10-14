# Construir desde Backend/ :  docker build -f maintenance-service/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY shared/ shared/
COPY maintenance-service/src/ maintenance-service/src/
RUN dotnet publish maintenance-service/src/Controllers/Controllers.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Maintenance.Api.dll"]
