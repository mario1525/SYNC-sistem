# Construir desde Backend/ :  docker build -f api-gateway/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY shared/ shared/
COPY api-gateway/ api-gateway/
RUN dotnet publish api-gateway/api-gateway.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
